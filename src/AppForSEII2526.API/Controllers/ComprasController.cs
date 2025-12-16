using AppForSEII2526.API.DTOs.CompraDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
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
        [ProducesResponseType(typeof(CompraBocadilloDetailDTO), (int)HttpStatusCode.OK)]
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
        .Select(c => new CompraBocadilloDetailDTO(
    c.CompraId,
    c.FechaCompra,
    c.BocadillosComprados.Sum(ci => ci.Bocadillo.PVP * ci.Cantidad),
    c.User.NombreCliente,
    c.User.Apellido1_Cliente,
    c.User.Apellido2_Cliente,

    c.MetodoPago.Id,                               
    c.BocadillosComprados.Sum(ci => ci.Cantidad), 

    c.BocadillosComprados
        .Select(ci => new CompraBocadilloItemDTO(
            ci.Bocadillo.Nombre,
            ci.Bocadillo.PVP,
            ci.Bocadillo.TipoPan.Nombre,
            ci.Cantidad
        ))
        .ToList()
)
)
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
            
            if (compraForCreate.MetodoPagoId<0)
                ModelState.AddModelError("MetodoPago", "El cliente debe escoger un método de pago");

            var user = _context.ApplicationUser.FirstOrDefault(au => au.NombreCliente == compraForCreate.NombreCliente);
            if (user == null)
                ModelState.AddModelError("CompraApplicationUser", "El usuario no está registrado");
            if (compraForCreate.BocadillosComprados.IsNullOrEmpty())
                ModelState.AddModelError("NoBocadillos", "Debe de seleccionar al menos un bocadillo para comprar");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            var nombresBocadillos = compraForCreate.BocadillosComprados
                .Select(b => b.Nombre)
                .Distinct()
                .ToList();
            /*var nombresBocadillos = compraForCreate.BocadillosComprados.Select(ci => ci.Nombre).ToList<string>();*/

            
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

            float precioTotal = 0;
            var metodoPago = _context.MetodoPago.FirstOrDefault(m => m.Id == compraForCreate.MetodoPagoId);

            var compra = new Compra(
               compraForCreate.NombreCliente,
               compraForCreate.Apellido1_cliente,
               compraForCreate.Apellido2_cliente,
               user,
               DateTime.Today,
               0,                              
               metodoPago,
               new List<CompraBocadillo>());



            foreach (var item in compraForCreate.BocadillosComprados)
            {
                var bocadillo = await _context.Bocadillo
                    .Include(b => b.TipoPan)
                    .FirstOrDefaultAsync(b => b.Nombre == item.Nombre);

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

                if(item.Cantidad > 5)
                {
                    ModelState.AddModelError("BocadillosComprados",
                        $"Error! no nos quedan panes para realizar tu pedido");
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

                precioTotal += bocadillo.PVP * item.Cantidad;
            }
            compra.nBocadillos = compra.BocadillosComprados.Sum(cb => cb.Cantidad);
            compra.PrecioTotal = precioTotal;
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

            
            compra.nBocadillos = compra.BocadillosComprados.Sum(cb => cb.Cantidad);
            compra.PrecioTotal = precioTotal;

            
            _context.Add(compra);

            try
            {
                Console.WriteLine("MetodoPago en BD: " + _context.MetodoPago.Count());
                Console.WriteLine("Bocadillos en BD: " + _context.Bocadillo.Count());
                Console.WriteLine("Users en BD: " + _context.ApplicationUser.Count()); 
                Console.WriteLine("MetodoPago dentro de compra:");
                Console.WriteLine(compra.MetodoPago == null ? "NULL" : $"Id={compra.MetodoPago.Id}");
                Console.WriteLine("MetodoPagoId enviado por DTO: " + compraForCreate.MetodoPagoId);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la compra");
                ModelState.AddModelError("Compra", "Error al guardar la compra.");
                //return Conflict("Error: " + ex.Message);
                Console.WriteLine("---- ERROR COMPLETO ----");
                Console.WriteLine(ex.ToString());

                if (ex.InnerException != null)
                {
                    Console.WriteLine("---- INNER ----");
                    Console.WriteLine(ex.InnerException.ToString());
                }

                throw;
            }


            var bocadillosDto = compra.BocadillosComprados
                .Select(cb => new CompraBocadilloItemDTO
                {
                    Nombre = cb.Bocadillo.Nombre,
                    TipoPan = cb.Bocadillo.TipoPan.Nombre, 
                    Precio = cb.Bocadillo.PVP,
                    Cantidad = cb.Cantidad
                })
                .ToList();
            var cantidadTotal = compra.BocadillosComprados.Sum(cb => cb.Cantidad);

            var compraDetail = new CompraBocadilloDetailDTO(
                compra.CompraId,
                compra.FechaCompra,
                compra.PrecioTotal,
                compra.User.NombreCliente,
                compra.User.Apellido1_Cliente,
                compra.User.Apellido2_Cliente,
                compra.MetodoPago.Id,
                cantidadTotal,
                bocadillosDto
                        );

            return CreatedAtAction("GetCompra", new { id = compra.CompraId }, compraDetail);
        }
    }
}
