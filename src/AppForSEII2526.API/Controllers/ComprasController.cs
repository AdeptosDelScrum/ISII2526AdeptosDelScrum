using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppForSEII2526.API.DTOs.CompraDTOs;
using Microsoft.IdentityModel.Tokens;
using AppForSEII2526.API.DTOs;
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

            var compra = await _context.Compra
             .Where(c => c.CompraId == id)
                 .Include(c => c.BocadillosComprados) 
                    .ThenInclude(ci => ci.Bocadillo) 
                        .ThenInclude(c => c.TipoPan) 
             .Select(c => new CompraBocadilloDetailDTO(c.CompraId,c.FechaCompra, c.PrecioTotal,
                                                        c.User.NombreCliente, c.User.Apellido1_Cliente, 
                                                        c.User.Apellido2_Cliente, c.nBocadillos,c.MetodoPago, 
                                                        c.BocadillosComprados
                                                        .Select(ci => new CompraBocadilloItemDTO(ci.Bocadillo.Nombre, 
                                                                                                ci.Precio, ci.TipoPan, ci.Cantidad)).ToList<CompraBocadilloItemDTO>()
                                                        ))
             .FirstOrDefaultAsync();


            if (compra == null)
            {
                _logger.LogError($"Error: La compra con Id {id} no existe");
                return NotFound();
            }


            return Ok(compra);
        }
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(CompraBocadilloDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CrearCompra(CompraBocadilloForCreateDTO compraForCreate)
        {
            if (compraForCreate.NombreCliente.IsNullOrEmpty())
                ModelState.AddModelError("NombreCliente", "El cliente debe ingrsar su nombre");

            if (compraForCreate.Apellido1_cliente.IsNullOrEmpty())
                ModelState.AddModelError("Apellido1Cliente", "El cliente debe ingrsar su primer apellido");

            if (compraForCreate.MetodoPago.Id == 0)
                ModelState.AddModelError("MetodoPago", "El cliente debe escoger un método de pago");

            var user = _context.ApplicationUser.FirstOrDefault(au => au.NombreCliente == compraForCreate.NombreCliente);
            if (user == null)
                ModelState.AddModelError("CompraApplicationUser", "El usuario no está registrado");
            if (compraForCreate.BocadillosComprados.IsNullOrEmpty())
                ModelState.AddModelError("NoBocadillos", "Debe de seleccionar al menos un bocadillo para comprar");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            var nombresBocadillos = compraForCreate.BocadillosComprados.Select(ci => ci.Nombre).ToList<string>();

            var bocadillos = _context.Bocadillo
                .Include(b => b.ComprasDelBocadillo)
                .ThenInclude(ci => ci.Compra)
                .Where(b => nombresBocadillos.Contains(b.Nombre))
                .ToList();
            /*
            var bocadillos = _context.Bocadillo.Include(c => c.ComprasDelBocadillo)
                .ThenInclude(ci => ci.Compra)
                .Where(c => nombresBocadillos.Contains(c.Nombre))

                .Select(c => new {
                    c.Id,
                    c.Nombre,
                    c.PVP,
                    c.Stock,
                    c.TamanyoBocadillo,
                    NumeroCompras = c.ComprasDelBocadillo.Count()
                })
                .ToList();*/

            float PrecioTotal = 0;
            Compra compra = new Compra(compraForCreate.NombreCliente, compraForCreate.Apellido1_cliente, compraForCreate.Apellido2_cliente, user,
                  DateTime.Now, compraForCreate.BocadillosComprados.Count, compraForCreate.MetodoPago, new List<CompraBocadillo>());
            /*
            Compra compra = new Compra(compraForCreate.NombreCliente, compraForCreate.Apellido1_cliente, compraForCreate.Apellido2_cliente,user,
                  DateTime.Now, compraForCreate.BocadillosComprados.Count,compraForCreate.MetodoPago, new List<CompraBocadillo>());
            */


            foreach (var item in compraForCreate.BocadillosComprados)
            {
                var bocadillo = bocadillos.FirstOrDefault(b => b.Nombre == item.Nombre);

                if (bocadillo == null)
                {
                    ModelState.AddModelError("BocadillosComprados", $"El bocadillo '{item.Nombre}' no existe.");
                    continue;
                }

                if (item.Cantidad <= 0)
                {
                    ModelState.AddModelError("BocadillosComprados",
                        $"Debe indicar una cantidad mayor que 0 para '{item.Nombre}'.");
                    continue;
                }

                
                if (item.Cantidad > bocadillo.Stock)
                {
                    ModelState.AddModelError("BocadillosComprados",
                        $"No hay stock suficiente de '{item.Nombre}'. Stock disponible: {bocadillo.Stock}.");
                    continue;
                }

                
                var compraBocadillo = new CompraBocadillo(bocadillo, item.Cantidad, compra);
                compra.BocadillosComprados.Add(compraBocadillo);

                PrecioTotal += bocadillo.PVP * item.Cantidad;
            }
            compra.nBocadillos = compra.BocadillosComprados.Sum(cb => cb.Cantidad);
            compra.PrecioTotal = PrecioTotal;
            /*
            foreach (var item in compraForCreate.BocadillosComprados)
            {
                var bocadillo = bocadillos.FirstOrDefault(m => m.Nombre == item.Nombre);
                
                if ((bocadillo == null) || (bocadillo.NumeroCompras >= bocadillo.Stock))
                {
                    ModelState.AddModelError("ItemsCompra", $"El bocadillo '{item.Nombre}' no está disponible");
                }
                else
                {
                    
                    compra.BocadillosComprados.Add(new CompraBocadillo(bocadillo,compra));
                    item.Precio = bocadillo.PVP;
                }
            }

            compra.PrecioTotal = compra.BocadillosComprados.Sum(ci => (ci.Precio * ci.Cantidad));*/


            
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Add(compra);

            try
            {
                
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Rental", $"Error! There was an error while saving your rental, plese, try again later");
                return Conflict("Error" + ex.Message);

            }

           
            var compraDetail = new CompraBocadilloDetailDTO(compra.CompraId, compra.FechaCompra,
                compra.PrecioTotal, compra.User.NombreCliente,
                compra.User.Apellido1_Cliente, compra.User.Apellido2_Cliente, compraForCreate.BocadillosComprados.Count(), compraForCreate.MetodoPago,
                
                compraForCreate.BocadillosComprados);

            return CreatedAtAction("GetRental", new { id = compra.CompraId }, compraDetail);
        }
    }
}
