using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using AppForSEII2526.API.DTOs;
using AppForSEII2526.Models;   // BonoBocadillo, TipoBocadillo
// using AppForSEII2526.Web.Data;  // si tu ApplicationDbContext está en ese namespace

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class BonoBocadilloController : ControllerBase   
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BonoBocadilloController> _logger;  

        public BonoBocadilloController(                               
            ApplicationDbContext context,
            ILogger<BonoBocadilloController> logger)                   
        {
            _context = context;
            _logger = logger;
        }

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
            }

            var bonos = await q
                .OrderBy(b => b.Nombre)
                .Select(b => new BonoBocadilloDTO
            
                    BonoId = b.BonoId,
                    Nombre = b.Nombre,
                    NBocadillos = b.NBocadillos,
                    CantidadDisponible = b.CantidadDisponible,
                    Pvp = b.PVP,
                    IdTipo = b.TipoBocadillo != null ? b.TipoBocadillo.IdTipo : 0,
                    NombreTipo = b.TipoBocadillo != null ? b.TipoBocadillo.NombreTipo : null
                })
                .ToListAsync();

            return Ok(bonos);
        }
    }
}
