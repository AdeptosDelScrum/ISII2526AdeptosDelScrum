using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppForSEII2526.API.DTOs.CompraDTOs;
namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ComprasController> _logger;

        public ComprasController(ApplicationDbContext context, ILogger<ComprasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraBocadilloForCreateDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetCompra(int id)
        {
            if (_context.Compra == null)
            {
                _logger.LogError("Error: La tabla Compra no existe");
                return NotFound();
            }

            var rental = await _context.Compra
             .Where(c => c.CompraId == id)
                 .Include(c => c.BocadillosComprados)
                    .ThenInclude(ci => ci.Bocadillo)
                        .ThenInclude(c => c.TipoPan)
             .Select(c => new CompraBocadilloDetailDTO(c.CompraId, c.FechaCompra, c.PrecioTotal,
                                                        c.NombreCliente, c.Apellido1_Cliente,
                                                        c.Apellido2_Cliente, c.MetodoPago,
                                                        c.BocadillosComprados
                                                        .Select(ci => new CompraBocadilloItemDTO(ci.Bocadillo.Nombre,
                                                                                                ci.Precio, ci.TipoPan,
                                                                                                ci.Cantidad)).ToList<CompraBocadilloItemDTO>()
                                                        ))
             .FirstOrDefaultAsync();


            if (rental == null)
            {
                _logger.LogError($"Error: La compra con Id {id} no existe");
                return NotFound();
            }


            return Ok(rental);
        }
    }
}
