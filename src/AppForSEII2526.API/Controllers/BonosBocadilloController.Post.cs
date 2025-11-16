using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AppForSEII2526.API.DTOs;
using AppForSEII2526.Models;               // BonoBocadillo, Compra, BonosComprados
using MetodoPagoEntity = AppForSEII2526.API.Models.MetodoPago; // ⚠️ MetodoPago (entidad) está en API.Models

namespace AppForSEII2526.API.Controllers
{
    public partial class BonosBocadilloController : ControllerBase
    {
        [HttpPost("compra")]
        [ProducesResponseType(typeof(CompraDetailsDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> PostCompra([FromBody] CrearCompraDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto?.Items == null || dto.Items.Count == 0)
                return BadRequest("Debe incluir al menos un bono.");

            // 1) Metodo de pago (ENTIDAD)
            var metodoPago = await _context.Set<MetodoPagoEntity>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m =>
                       EF.Property<long>(m, "MetodoPagoId") == dto.MetodoPagoId
                    || EF.Property<long>(m, "Id") == dto.MetodoPagoId
                , ct);

            if (metodoPago == null)
                return BadRequest("Metodo de pago invalido.");

            // 2) Normalizar items
            var itemsAgrupados = dto.Items
                .GroupBy(i => i.BonoId)
                .Select(g => new { BonoId = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
                .ToList();

            // ⚠️ En tu entity BonoBocadillo.BonoId es int -> convierte
            var ids = itemsAgrupados.Select(x => (int)x.BonoId).ToList();

            var bonos = await _context.Set<BonoBocadillo>()
                .Include(b => b.TipoBocadillo)
                .Where(b => ids.Contains(b.BonoId))
                .ToListAsync(ct);

            // 3) Validaciones
            var encontrados = bonos.Select(b => b.BonoId).ToHashSet();
            var faltantes = ids.Where(id => !encontrados.Contains(id)).ToList();
            if (faltantes.Any())
                return BadRequest($"Bono(s) inexistente(s): {string.Join(",", faltantes)}");

            foreach (var it in itemsAgrupados)
            {
                var b = bonos.First(x => x.BonoId == (int)it.BonoId);
                if (it.Cantidad < 1)
                    return BadRequest($"Cantidad invalida para bono {b.BonoId}.");
                if (b.CantidadDisponible < it.Cantidad)
                    return Conflict($"Stock insuficiente para bono {b.Nombre} (disponible: {b.CantidadDisponible}, pedido: {it.Cantidad}).");
            }

            using var tx = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                // 4) Cabecera
                var compra = new Compra
                {
                    // ⚠️ Usa el nombre REAL de tu propiedad:
                    // Si tu entity tiene NombreCompleto:
                    //   NombreCompleto = dto.NombreCompleto ?? "",
                    // Si tiene NombreCliente (común):
                    //   NombreCliente = dto.NombreCompleto ?? "",
                    FechaCompra = DateTime.UtcNow,
                    // ⚠️ Asigna la FK o la navegación:
                    // Si tu entity tiene MetodoPagoId (int/long):
                    //   MetodoPagoId = (int)dto.MetodoPagoId,
                    // Si tu entity tiene la navegación MetodoPago:
                    //   MetodoPago = metodoPago,
                    // ⚠️ Inicializa la colección con el NOMBRE REAL:
                    // BonosComprados = new List<BonosComprados>()
                };

                // ---------- INICIO: asignaciones dependientes de tu modelo ----------
                // Nombre:
                var pNombreCliente = compra.GetType().GetProperty("NombreCliente");
                var pNombreCompleto = compra.GetType().GetProperty("NombreCompleto");
                pNombreCliente?.SetValue(compra, dto.NombreCompleto ?? "");
                pNombreCompleto?.SetValue(compra, dto.NombreCompleto ?? "");

                // MetodoPagoId (FK) preferente si existe
                var pMetodoPagoIdLong = compra.GetType().GetProperty("MetodoPagoId");
                if (pMetodoPagoIdLong != null)
                {
                    if (pMetodoPagoIdLong.PropertyType == typeof(long))
                        pMetodoPagoIdLong.SetValue(compra, dto.MetodoPagoId);
                    else if (pMetodoPagoIdLong.PropertyType == typeof(int))
                        pMetodoPagoIdLong.SetValue(compra, unchecked((int)dto.MetodoPagoId));
                }
                else
                {
                    // Si no hay FK, intenta la navegación
                    var pMetodoPago = compra.GetType().GetProperty("MetodoPago");
                    pMetodoPago?.SetValue(compra, metodoPago);
                }

                // Colección de líneas
                var pLineas = compra.GetType().GetProperty("BonosComprados")
                             ?? compra.GetType().GetProperty("Detalles"); // por si se llama así
                if (pLineas != null && pLineas.GetValue(compra) == null)
                {
                    // crea lista del tipo correcto
                    var listType = typeof(List<>).MakeGenericType(pLineas.PropertyType.GenericTypeArguments[0]);
                    pLineas.SetValue(compra, Activator.CreateInstance(listType));
                }
                // ---------- FIN: asignaciones dependientes de tu modelo ----------

                decimal total = 0m;

                // 5) Líneas + stock
                foreach (var it in itemsAgrupados)
                {
                    var b = bonos.First(x => x.BonoId == (int)it.BonoId);

                    // crea instancia de linea (BonosComprados o Detalle)
                    var lineType = pLineas!.PropertyType.GenericTypeArguments[0];
                    var linea = Activator.CreateInstance(lineType)!;

                    // set BonoId
                    var pBonoId = lineType.GetProperty("BonoId")
                                 ?? lineType.GetProperty("BonoBocadilloId"); // fallback
                    pBonoId?.SetValue(linea, b.BonoId);

                    // set Cantidad (obligatorio)
                    lineType.GetProperty("Cantidad")?.SetValue(linea, it.Cantidad);

                    // si tu linea tiene PrecioUnitario, lo seteamos
                    var pPrecioUnit = lineType.GetProperty("PrecioUnitario");
                    pPrecioUnit?.SetValue(linea, b.PVP);

                    // añade a la colección
                    var lista = (System.Collections.IList)pLineas.GetValue(compra)!;
                    lista.Add(linea);

                    // stock y total
                    b.CantidadDisponible -= it.Cantidad;
                    total += b.PVP * it.Cantidad;
                }

                _context.Set<Compra>().Add(compra);
                await _context.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                // 6) DTO respuesta (no dependemos de nombres raros de la entity)
                var itemsDto = new List<CompraBonoItemDTO>();
                foreach (var it in itemsAgrupados)
                {
                    var b = bonos.First(x => x.BonoId == (int)it.BonoId);
                    itemsDto.Add(new CompraBonoItemDTO
                    {
                        BonoId = b.BonoId,
                        Nombre = b.Nombre,
                        Tipo = b.TipoBocadillo?.NombreTipo,
                        Pvp = b.PVP,
                        Cantidad = it.Cantidad
                    });
                }

                var result = new CompraDetailsDTO
                {
                    CompraId = (long)compra.GetType().GetProperty("CompraId")!.GetValue(compra)!,
                    NombreCompleto = dto.NombreCompleto ?? "",
                    Apellidos = "", // tu entity no lo tiene; mantenemos DTO limpio
                    MetodoPago = (metodoPago.GetType().GetProperty("Nombre")?.GetValue(metodoPago)?.ToString())
                                     ?? (metodoPago.GetType().GetProperty("Descripcion")?.GetValue(metodoPago)?.ToString())
                                     ?? $"MetodoPago:{dto.MetodoPagoId}",
                    Fecha = (DateTime)compra.GetType().GetProperty("FechaCompra")!.GetValue(compra)!,
                    PrecioTotal = itemsDto.Sum(i => i.Pvp * i.Cantidad),
                    Items = itemsDto
                };

                // evita dependencia de GetCompraDetails si no existe
                return Created($"/api/bonosbocadillo/compra/{result.CompraId}", result);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Conflicto de concurrencia al actualizar el stock.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la compra");
                return Problem("Error inesperado al crear la compra.");
            }
        }
    }
}

