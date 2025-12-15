using AppForSEII2526.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BocadillosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BocadillosController> _logger;

        public BocadillosController(ApplicationDbContext context, ILogger<BocadillosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof (IList<Bocadillo>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetBocadillosParaCompra(Tamanyo? TamanyoBocadillo, string?TipoPanBocadillo)
        {

            if (!string.IsNullOrEmpty(TipoPanBocadillo) && !_context.Bocadillo.Any(c => c.TipoPan.Nombre.ToLower() == TipoPanBocadillo.ToLower()))
            {
                ModelState.AddModelError("TipoPan", "No se ha encontrado ese tipo de pan");
                _logger.LogError($"{DateTime.Now} Error: No se ha encontrado ese tipo de pan");
                return BadRequest(new ValidationProblemDetails(ModelState));
                //return BadRequest("No se ha encontrado ese tipo de pan");
            }

            IList<BocadilloDTO> bocadillos = await _context.Bocadillo
            .Where(b =>
                (TipoPanBocadillo == null || b.TipoPan.Nombre.Contains(TipoPanBocadillo)) &&
                (TamanyoBocadillo == null || b.TamanyoBocadillo == TamanyoBocadillo)
            )
            .OrderBy(b => b.Nombre)
            .Select(b => new BocadilloDTO(
                b.Nombre,
                b.TamanyoBocadillo,
                b.TipoPan.Nombre,
                b.PVP
            ))
            .ToListAsync();

            return Ok(bocadillos);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<BocadilloDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetBocadillosParaResenya(string? nombre, float? PVP)
        {
            if (PVP != null && PVP < 0)
            {
                return BadRequest("El PVP no puede ser negativo");
            }
            var bocadillos = await _context.Bocadillo
                .Where(b =>
                (nombre == null || b.Nombre.Contains(nombre)) &&
                (PVP == null || b.PVP <= PVP))
                .OrderBy(b => b.Nombre)
                .Select(b => new BocadilloDTO(b.Id,b.Nombre, b.TamanyoBocadillo, b.TipoPan.Nombre, b.PVP))
                .ToListAsync();
            return Ok(bocadillos);
        }

    }
}
