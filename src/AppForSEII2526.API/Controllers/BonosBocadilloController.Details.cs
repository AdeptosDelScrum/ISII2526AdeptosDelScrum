using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using AppForSEII2526.API.DTOs;
using AppForSEII2526.Models; // Compra, BonosComprados, BonoBocadillo, TipoBocadillo, MetodoPago

namespace AppForSEII2526.API.Controllers
{
    // MUY IMPORTANTE: el nombre de la clase debe coincidir EXACTAMENTE con el de los otros parciales.
    public partial class BonosBocadilloController : ControllerBase
    {
        // GET: api/bonosbocadillo/compra/{id}/details
        [HttpGet("compra/{id:long}/details")]
        [ProducesResponseType(typeof(CompraDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCompraDetails(long id, CancellationToken ct = default)
        {
            // 1) Cabecera (sin referenciar props que no existan en compile-time)
            var compra = await _context.Set<Compra>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => EF.Property<long>(c, "CompraId") == id, ct);

            if (compra == null) return NotFound();

            // Campos de cabecera (intenta varios nombres comunes con EF.Property)
            string nombreCompleto = "";
            string apellidos = "";
            DateTime fecha = DateTime.MinValue;
            string metodoPagoNombre = "";

            try { nombreCompleto = EF.Property<string>(compra, "NombreCompleto") ?? ""; } catch { }
            try { apellidos = EF.Property<string>(compra, "Apellidos") ?? ""; } catch { }
            try { fecha = EF.Property<DateTime>(compra, "FechaCompra"); } catch { }
            if (fecha == DateTime.MinValue)
            {
                try { fecha = EF.Property<DateTime>(compra, "Fecha"); } catch { }
            }

            // Metodo de pago: primero intenta string directo; si no, resuelve por FK MetodoPagoId
            try { metodoPagoNombre = EF.Property<string>(compra, "MetodoPago") ?? ""; } catch { }
            if (string.IsNullOrWhiteSpace(metodoPagoNombre))
            {
                try
                {
                    int? metodoPagoId = EF.Property<int?>(compra, "MetodoPagoId");
                    if (metodoPagoId.HasValue)
                    {
                        var mp = await _context.Set<MetodoPago>()
                            .AsNoTracking()
                            .FirstOrDefaultAsync(m => EF.Property<int>(m, "Id") == metodoPagoId.Value, ct);

                        if (mp != null)
                        {
                            try { metodoPagoNombre = EF.Property<string>(mp, "Nombre") ?? metodoPagoId.Value.ToString(); }
                            catch { metodoPagoNombre = metodoPagoId.Value.ToString(); }
                        }
                    }
                }
                catch { /* si no hay ni MetodoPago ni MetodoPagoId, se deja vacío */ }
            }

            // 2) Lineas: usa joins (no requiere navegaciones en BonosComprados)
            var items = await (
                from l in _context.Set<BonosComprados>().AsNoTracking()
                where EF.Property<long>(l, "CompraId") == id
                join b in _context.Set<BonoBocadillo>().AsNoTracking()
                    on EF.Property<int>(l, "BonoId") equals b.BonoId
                join t in _context.Set<TipoBocadillo>().AsNoTracking()
                    on b.IdTipo equals t.IdTipo into tj
                from t in tj.DefaultIfEmpty()
                select new CompraBonoItemDTO
                {
                    BonoId = b.BonoId,
                    Nombre = b.Nombre,
                    Tipo = t != null ? t.NombreTipo : null,
                    Pvp = b.PVP,                                  // en tu entidad es PVP (mayúsculas)
                    Cantidad = EF.Property<int>(l, "Cantidad")          // ajusta si tu columna se llama distinto
                }
            ).ToListAsync(ct);

            // 3) DTO final
            var dto = new CompraDetailsDTO
            {
                CompraId = id,
                NombreCompleto = nombreCompleto,
                Apellidos = apellidos,
                MetodoPago = metodoPagoNombre,
                Fecha = fecha == DateTime.MinValue ? DateTime.UtcNow : fecha,
                PrecioTotal = items.Sum(i => i.Pvp * i.Cantidad),
                Items = items
            };

            return Ok(dto);
        }
    }
}

