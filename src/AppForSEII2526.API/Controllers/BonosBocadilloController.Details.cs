using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.DTOs;

namespace AppForSEII2526.API.Controllers
{
    public partial class BonosBocadilloController : ControllerBase
    {
        // GET: api/bonosbocadillo/compra/{id}/details
        // Historia 7: cabecera (nombre, apellidos, metodoPago, fecha, total)
        // y por bono (nombre, tipo, precio, cantidad).
        [HttpGet("compra/{id:long}/details")]
        [ProducesResponseType(typeof(CompraDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCompraDetails(long id, CancellationToken ct = default)
        {
            var compra = await _context.Compra
                .AsNoTracking()
                .Include(c => c.Detalles)                                   // lineas de la compra
                    .ThenInclude(d => d.BonoBocadillo)                      // bono comprado
                        .ThenInclude(b => b.TipoBocadillo)                  // tipo del bono
                .FirstOrDefaultAsync(c => c.CompraId == id, ct);

            if (compra == null) return NotFound();

            // si tu modelo guarda PrecioUnitario en la linea, usalo; si no, usa b.Pvp
            var items = compra.Detalles.Select(d =>
            {
                var b = d.BonoBocadillo;
                var precioUnit = d.PrecioUnitario ?? b.Pvp;
                return new CompraBonoItemDTO
                {
                    BonoId   = b.BonoId,
                    Nombre   = b.Nombre,
                    Tipo     = b.TipoBocadillo != null ? b.TipoBocadillo.NombreTipo : null,
                    Pvp      = precioUnit,
                    Cantidad = d.Cantidad
                };
            }).ToList();

            var dto = new CompraDetailsDTO
            {
                CompraId       = compra.CompraId,
                NombreCompleto = compra.NombreCompleto,
                Apellidos      = compra.Apellidos,
                MetodoPago     = compra.MetodoPago,
                Fecha          = compra.FechaCompra,
                PrecioTotal    = items.Sum(i => i.Pvp * i.Cantidad),
                Items          = items
            };

            return Ok(dto);
        }
    }
}
