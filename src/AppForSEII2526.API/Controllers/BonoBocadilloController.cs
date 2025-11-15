using AppForSEII2526.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BonosBocadilloController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BonosBocadilloController> _logger;

        public BonosBocadilloController(ApplicationDbContext context, ILogger<BonosBocadilloController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/bonosbocadillo/GetBonosDisponiblesSelect?tipo=vegano&search=mixto
        // Devuelve solo bonos con stock (>0) mostrando nombre, pvp, nBocadillos y tipo.
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<BonoBocadilloDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetBonosDisponiblesSelect(string? tipo = null, string? search = null)
        {
            var q = _context.BonoBocadillo
                .AsNoTracking()
                .Include(b => b.TipoBocadillo)
                .Where(b => b.CantidadDisponible > 0);

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                var t = tipo.Trim().ToLower();
                q = q.Where(b => b.TipoBocadillo != null &&
                                 b.TipoBocadillo.NombreTipo.ToLower() == t);
                // valores esperados: vegano | vegetariano | sin gluten | normal
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(b => b.Nombre.Contains(s));
            }

            var bonos = await q
                .OrderBy(b => b.Nombre)
                .Select(b => new BonoBocadilloDTO
                {
                    BonoId = b.BonoId,
                    Nombre = b.Nombre,
                    NBocadillos = b.NBocadillos,
                    CantidadDisponible = b.CantidadDisponible, // no se muestra en UI, pero disponible
                    Pvp = b.Pvp,
                    Tipo = b.TipoBocadillo == null ? null : new TipoBocadilloDTO
                    {
                        IdTipo = b.TipoBocadillo.IdTipo,
                        NombreTipo = b.TipoBocadillo.NombreTipo
                    }
                })
                .ToListAsync();

            return Ok(bonos);
        }
    }
}
