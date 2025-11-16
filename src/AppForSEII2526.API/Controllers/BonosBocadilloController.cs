using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AppForSEII2526.API.DTOs;   // BonoBocadilloDTO
using AppForSEII2526.Models;     // BonoBocadillo, TipoBocadillo

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class BonosBocadilloController : ControllerBase
    {
        private readonly ApplicationDbContext _context;                 // usa tu DbContext real
        private readonly ILogger<BonosBocadilloController> _logger;

        public BonosBocadilloController(
            ApplicationDbContext context,
            ILogger<BonosBocadilloController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/bonosbocadillo/GetBonosDisponiblesSelect?tipo=vegano&search=mixto
        [HttpGet("[action]")]
        [ProducesResponseType(typeof(IList<BonoBocadilloDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetBonosDisponiblesSelect(string? tipo = null, string? search = null)
        {
            IQueryable<BonoBocadillo> q = _context.Set<BonoBocadillo>()
                .AsNoTracking()
                .Include(b => b.TipoBocadillo)
                .Where(b => b.CantidadDisponible > 0);

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                var t = tipo.Trim().ToLower();
                q = q.Where(b => b.TipoBocadillo != null &&
                                 b.TipoBocadillo.NombreTipo.ToLower() == t);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                q = q.Where(b => b.Nombre.Contains(s));
                // Alternativa: q = q.Where(b => EF.Functions.Like(b.Nombre, $"%{s}%"));
            }

            var bonos = await q
                .OrderBy(b => b.Nombre)
                .Select(b => new BonoBocadilloDTO
                {
                    BonoId = b.BonoId,
                    Nombre = b.Nombre,
                    NBocadillos = b.NBocadillos,
                    CantidadDisponible = b.CantidadDisponible, // quítalo si no quieres exponerlo
                    Pvp = b.PVP,                // en la entidad es PVP
                    IdTipo = b.TipoBocadillo != null ? b.TipoBocadillo.IdTipo : 0,
                    NombreTipo = b.TipoBocadillo != null ? b.TipoBocadillo.NombreTipo : null
                })
                .ToListAsync();

            return Ok(bonos);
        }
    }
}

