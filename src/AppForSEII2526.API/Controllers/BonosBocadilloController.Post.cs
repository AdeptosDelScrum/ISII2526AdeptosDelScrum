using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using AppForSEII2526.API.DTOs;
using AppForSEII2526.Models; // Compra, BonosComprados, BonoBocadillo, MetodoPago, TipoBocadillo

namespace AppForSEII2526.API.Controllers
{
    public partial class BonosBocadilloController : ControllerBase
    {
        // POST: api/bonosbocadillo/compra
        // Body: CrearCompraDTO
        // Respuesta: CompraDetailsDTO (resumen de la compra creada)
        [HttpPost("compra")]
        [ProducesResponseType(typeof(CompraDetailsDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> PostCompra([FromBody] CrearCompraDTO dto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest("Debe incluir al menos un bono.");

            // 1) Metodo de pago (DTO trae long). Resolvemos en memoria para evitar choques int/long.
            var metodos = await _context.Set<MetodoPago>()
                .AsNoTracking()
                .ToListAsync(ct);

            var metodoPago = metodos.FirstOrDefault(m => GetIdAsLong(m) == dto.MetodoPagoId);
            if (metodoPago == null)
                return BadRequest("Metodo de pago invalido.");

            // 2) Normalizar items (agrupar duplicados) y convertir los IDs a int (tu entidad usa int)
            var itemsPorBono = dto.Items
                .GroupBy(i => i.BonoId)
                .Select(g => new
                {
                    BonoIdInt = checked((int)g.Key),   // arregla CS1503 (long->int)
                    Cantidad = g.Sum(x => x.Cantidad)
                })
                .ToList();

            var ids = itemsPorBono.Select(i => i.BonoIdInt).ToList();

            // 3) Bonos
            var bonos = await _context.Set<BonoBocadillo>()
                .Include(b => b.TipoBocadillo)
                .Where(b => ids.Contains(b.BonoId))
                .ToListAsync(ct);

            // 3.a) Validaciones de existencia y stock
            var idsEncontrados = bonos.Select(b => b.BonoId).ToHashSet();
            var faltantes = ids.Where(id => !idsEncontrados.Contains(id)).ToList();
            if (faltantes.Any())
                return BadRequest($"Bono(s) inexistente(s): {string.Join(",", faltantes)}");

            foreach (var it in itemsPorBono)
            {
                var b = bonos.First(x => x.BonoId == it.BonoIdInt);
                if (it.Cantidad < 1)
                    return BadRequest($"Cantidad invalida para bono {b.BonoId}.");
                if (b.CantidadDisponible < it.Cantidad)
                    return Conflict($"Stock insuficiente para bono {b.Nombre} (disp: {b.CantidadDisponible}, pedido: {it.Cantidad}).");
            }

            using var tx = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                // 4) Cabecera de compra (solo seteamos propiedades que existan)
                var compra = new Compra();

                TrySet(compra, "FechaCompra", DateTime.UtcNow);
                TrySet(compra, "Fecha", DateTime.UtcNow); // por si tu campo se llama "Fecha"

                TrySet(compra, "NombreCompleto", dto.NombreCompleto);
                TrySet(compra, "Apellidos", dto.Apellidos);

                // MetodoPago: primero intentamos FK, luego navegación
                var metodoPagoIdLong = GetIdAsLong(metodoPago) ?? 0L;
                // Si la FK es long:
                if (!TrySet(compra, "MetodoPagoId", metodoPagoIdLong))
                {
                    // Si la FK es int:
                    TrySet(compra, "MetodoPagoId", checked((int)metodoPagoIdLong));
                }
                // Si tu modelo usa navegación en lugar de FK:
                TrySet(compra, "MetodoPago", metodoPago);

                _context.Add(compra);
                await _context.SaveChangesAsync(ct); // para obtener CompraId

                // 5) Lineas: tabla intermedia BonosComprados
                var lineas = new List<BonosComprados>();
                foreach (var it in itemsPorBono)
                {
                    var b = bonos.First(x => x.BonoId == it.BonoIdInt);
                    var precioUnit = b.PVP; // congelamos el PVP actual por línea

                    var l = new BonosComprados();
                    TrySet(l, "CompraId", compra.CompraId);
                    TrySet(l, "BonoId", b.BonoId);                // int
                    TrySet(l, "BonoId", (long)b.BonoId);          // por si la columna fuese long
                    TrySet(l, "Cantidad", it.Cantidad);
                    TrySet(l, "PrecioUnitario", precioUnit);      // si existe el campo en tu entidad

                    lineas.Add(l);

                    // actualizar stock
                    b.CantidadDisponible -= it.Cantidad;
                    _context.Update(b);
                }

                _context.AddRange(lineas);
                await _context.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                // 6) Construir DTO de respuesta
                var itemsDto = lineas.Select(l =>
                {
                    var bonoIdLinea = (int)(GetAsLong(l, "BonoId") ?? 0L);
                    var b = bonos.First(x => x.BonoId == bonoIdLinea);

                    var cant = GetIfExists<int>(l, "Cantidad");
                    decimal pUnit = GetIfExists<decimal>(l, "PrecioUnitario");
                    if (pUnit <= 0) pUnit = b.PVP;

                    return new CompraBonoItemDTO
                    {
                        BonoId = b.BonoId,
                        Nombre = b.Nombre,
                        Tipo = b.TipoBocadillo?.NombreTipo,
                        Pvp = pUnit,
                        Cantidad = cant
                    };
                }).ToList();

                string metodoPagoNombre = "";
                try { metodoPagoNombre = metodoPago.GetType().GetProperty("Nombre")?.GetValue(metodoPago)?.ToString() ?? ""; } catch { }

                var result = new CompraDetailsDTO
                {
                    CompraId = compra.CompraId,
                    NombreCompleto = GetIfExists<string>(compra, "NombreCompleto") ?? "",
                    Apellidos = GetIfExists<string>(compra, "Apellidos") ?? "",
                    MetodoPago = metodoPagoNombre,
                    Fecha = GetIfExists<DateTime>(compra, "FechaCompra") != default
                        ? GetIfExists<DateTime>(compra, "FechaCompra")
                        : (GetIfExists<DateTime>(compra, "Fecha") != default
                            ? GetIfExists<DateTime>(compra, "Fecha")
                            : DateTime.UtcNow),
                    PrecioTotal = itemsDto.Sum(i => i.Pvp * i.Cantidad),
                    Items = itemsDto
                };

                var location = $"/api/bonosbocadillo/compra/{compra.CompraId}/details";
                return Created(location, result);
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(ct);
                return Conflict("Conflicto de concurrencia al actualizar el stock.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(ct);
                // _logger puede ser null en tests; por seguridad comprobamos
                try { _logger?.LogError(ex, "Error al crear la compra"); } catch { }
                return Problem("Error inesperado al crear la compra.");
            }
        }

        // ---------- Helpers reflexión tip-safe ----------

        private static bool TrySet<T>(object entity, string propName, T value)
        {
            var p = entity.GetType().GetProperty(propName);
            if (p != null && p.CanWrite)
            {
                try
                {
                    if (value == null)
                    {
                        if (p.PropertyType.IsClass || Nullable.GetUnderlyingType(p.PropertyType) != null)
                        {
                            p.SetValue(entity, null);
                            return true;
                        }
                        return false;
                    }

                    var targetType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                    var converted = Convert.ChangeType(value, targetType);
                    p.SetValue(entity, converted);
                    return true;
                }
                catch { /* ignorar para probar otro prop/overload */ }
            }
            return false;
        }

        private static T GetIfExists<T>(object entity, string propName)
        {
            var p = entity.GetType().GetProperty(propName);
            if (p != null && p.CanRead)
            {
                try
                {
                    var val = p.GetValue(entity);
                    if (val == null) return default!;
                    return (T)Convert.ChangeType(val, typeof(T));
                }
                catch { }
            }
            return default!;
        }

        private static long? GetIdAsLong(object entity)
        {
            // intenta propiedades comunes para PK
            foreach (var name in new[] { "Id", "ID", "MetodoPagoId" })
            {
                var pi = entity.GetType().GetProperty(name);
                if (pi == null || !pi.CanRead) continue;
                try
                {
                    var val = pi.GetValue(entity);
                    if (val == null) continue;
                    return Convert.ToInt64(val);
                }
                catch { }
            }
            return null;
        }

        private static long? GetAsLong(object entity, string propName)
        {
            var pi = entity.GetType().GetProperty(propName);
            if (pi == null || !pi.CanRead) return null;
            try
            {
                var val = pi.GetValue(entity);
                if (val == null) return null;
                return Convert.ToInt64(val);
            }
            catch { return null; }
        }
    }
}
