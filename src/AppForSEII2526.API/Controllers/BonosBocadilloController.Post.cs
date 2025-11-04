using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.DTOs;

namespace AppForSEII2526.API.Controllers
{
    public partial class BonosBocadilloController : ControllerBase
    {
        // POST: api/bonosbocadillo/compra
        // Body: CrearCompraDTO
        // Respuesta: CompraDetailsDTO (resumen de la compra realizada)
        [HttpPost("compra")]
        [ProducesResponseType(typeof(CompraDetailsDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> PostCompra([FromBody] CrearCompraDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest("Debe incluir al menos un bono.");

            // Metodo de pago (existe una clase/tabla para ello)
            var metodoPago = await _context.MetodosPago
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == dto.MetodoPagoId, ct);

            if (metodoPago == null)
                return BadRequest("Metodo de pago invalido.");

            // Normalizar items por si repiten BonoId: agrupa y suma cantidades
            var itemsPorBono = dto.Items
                .GroupBy(i => i.BonoId)
                .Select(g => new { BonoId = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
                .ToList();

            var ids = itemsPorBono.Select(i => i.BonoId).ToList();

            var bonos = await _context.BonoBocadillo
                .Include(b => b.TipoBocadillo)
                .Where(b => ids.Contains(b.BonoId))
                .ToListAsync(ct);

            // Validaciones de existencia y stock
            var idsEncontrados = bonos.Select(b => b.BonoId).ToHashSet();
            var faltantes = ids.Where(id => !idsEncontrados.Contains(id)).ToList();
            if (faltantes.Any())
                return BadRequest($"Bono(s) inexistente(s): {string.Join(",", faltantes)}");

            foreach (var it in itemsPorBono)
            {
                var b = bonos.First(x => x.BonoId == it.BonoId);
                if (it.Cantidad < 1)
                    return BadRequest($"Cantidad invalida para bono {b.BonoId}.");
                if (b.CantidadDisponible < it.Cantidad)
                    return Conflict($"Stock insuficiente para bono {b.Nombre} (disponible: {b.CantidadDisponible}, pedido: {it.Cantidad}).");
            }

            using var tx = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                // Crear cabecera de compra (ajusta propiedades a tu entidad real)
                var compra = new Compra
                {
                    NombreCompleto = dto.NombreCompleto,
                    Apellidos = dto.Apellidos,
                    MetodoPago = metodoPago.Nombre, // o MetodoPagoId = metodoPago.Id si tu modelo usa FK
                    FechaCompra = DateTime.UtcNow,
                    Detalles = new List<CompraDetalle>()
                };

                decimal total = 0m;

                foreach (var it in itemsPorBono)
                {
                    var b = bonos.First(x => x.BonoId == it.BonoId);

                    // precio unitario "congelado" en la linea (recomendado)
                    var precioUnit = b.Pvp;

                    compra.Detalles.Add(new CompraDetalle
                    {
                        BonoBocadilloId = b.BonoId,
                        Cantidad = it.Cantidad,
                        PrecioUnitario = precioUnit
                    });

                    // actualizar stock
                    b.CantidadDisponible -= it.Cantidad;

                    total += precioUnit * it.Cantidad;
                }

                // si tu entidad Compra tiene campo Total, puedes guardarlo
                // compra.Total = total;

                _context.Compra.Add(compra);
                await _context.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                // Construir DTO de respuesta (con nombres/tipos)
                var itemsDto = compra.Detalles.Select(d =>
                {
                    var b = bonos.First(x => x.BonoId == d.BonoBocadilloId);
                    return new CompraBonoItemDTO
                    {
                        BonoId = b.BonoId,
                        Nombre = b.Nombre,
                        Tipo = b.TipoBocadillo?.NombreTipo,
                        Pvp = d.PrecioUnitario ?? b.Pvp,
                        Cantidad = d.Cantidad
                    };
                }).ToList();

                var result = new CompraDetailsDTO
                {
                    CompraId = compra.CompraId,
                    NombreCompleto = compra.NombreCompleto,
                    Apellidos = compra.Apellidos,
                    MetodoPago = compra.MetodoPago,
                    Fecha = compra.FechaCompra,
                    PrecioTotal = itemsDto.Sum(i => i.Pvp * i.Cantidad),
                    Items = itemsDto
                };

                // Devuelve 201 con ubicacion del details
                return CreatedAtAction(
                    nameof(GetCompraDetails),                    // metodo GET que ya tienes
                    new { id = compra.CompraId },               // route values
                    result);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(ct);
                return Conflict("Conflicto de concurrencia al actualizar el stock.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(ct);
                _logger.LogError(ex, "Error al crear la compra");
                return Problem("Error inesperado al crear la compra.");
            }
        }
    }
}
