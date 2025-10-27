using AppForSEII2526.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(typeof(BocadilloDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetBocadillosParaResenyar(string? nombre, float? PVP)
        {
            if (PVP < 0)
            {

            }
            var bocadillos = await _context.Bocadillo
                .Where(b=>(b.Nombre.Contains(nombre) && b.PVP.Equals(PVP)))
                .Select(b=>new BocadilloDTO(b.Id,b.Nombre,b.TipoPan.Nombre,b.PVP))
                .ToListAsync();
            return Ok(bocadillos);
        }
        
    }
}
