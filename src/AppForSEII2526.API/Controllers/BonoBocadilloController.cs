using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.Models;   // entidades: BonoBocadillo, TipoBocadillo
    

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class BonosBocadilloController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BonosBocadilloController> _logger;

        public BonosBocadilloController(ApplicationDbContext context, ILogger<BonosBocadilloController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/bonosbocadillo/GetBonosDisponiblesSelect?tipo=vegano&search=mixto
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<BonoBocadilloDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetBonosDisponiblesSelect(string? tipo = null, string? search = null)
        {
            // usar Set<T>() evita depender del nombre exacto del DbSet en el contexto
            IQueryable<BonoBocadillo> q = _context.Set<BonoBocadillo>()
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
                // alternativa: q = q.Where(b => EF.Functions.Like(b.Nombre, $"%{s}%"));
            }

            var bonos = await q
                .OrderBy(b => b.Nombre)
                .Select(b => new BonoBocadilloDTO
                {
                    BonoId = b.BonoId,
                    Nombre = b.Nombre,
                    NBocadillos = b.NBocadillos,
                    CantidadDisponible = b.CantidadDisponible, // quita si no quieres exponerlo
                    Pvp = b.PVP,
                    IdTipo = b.TipoBocadillo != null ? b.TipoBocadillo.IdTipo : 0,
                    NombreTipo = b.TipoBocadillo != null ? b.TipoBocadillo.NombreTipo : null
                })
                .ToListAsync();

            return Ok(bonos);
        }
    }
}
