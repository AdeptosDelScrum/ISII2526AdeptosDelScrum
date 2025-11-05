using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppForSEII2526.API.DTOs.CompraDTOs;
using Microsoft.IdentityModel.Tokens;
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

            if (compraForCreate.MetodoPago.Id.Equals(0))
                ModelState.AddModelError("MetodoPago", "El cliente debe escoger un método de pago");

            if (compraForCreate.BocadillosComprados.Count == 0)
                ModelState.AddModelError("BocadillosComprados", "Error! Debes incluir como mínimo un bocadillo para continuar");

            
            var user = _context.ApplicationUser.FirstOrDefault(au => au.NombreCliente == compraForCreate.NombreCliente);
            if (user == null)
                ModelState.AddModelError("CompraApplicationUser", "El usuario no está registrado");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            var bocadillos = compraForCreate.BocadillosComprados.Select(ci => ci.Nombre).ToList<string>();

            var movies = _context.Bocadillo.Include(m => m.ComprasDelBocadillo)
                .ThenInclude(ci => ci.Compra)
                .Where(b => bocadillos.Contains(b.Nombre))

                
                .Select(b => new {
                    b.Id,
                    b.Nombre,
                    b.ComprasDelBocadillo,
                    b.PVP,
                    //we count the number of rentalItems that are within the rental period
                    NumberOfRentedItems = b.ComprasDelBocadillo.Count(ci => ci.Compra.BocadillosComprados <= compraForCreate.BocadillosComprados
                            && ci.Compra.FechaCompra >= compraForCreate.)
                })
                .ToList();


            Compra compra = new Compra(compraForCreate.NombreCliente, compraForCreate.Apellido1_cliente, compraForCreate.Apellido2_cliente,
                  DateTime.Now,
                compraForCreate.MetodoPago,
                rentalForCreate.RentalDateFrom, rentalForCreate.RentalDateTo, new List<RentalItem>());


            compra.TotalPrice = 0;
            var numDays = (rental.RentalDateTo - rental.RentalDateFrom).TotalDays;


            foreach (var item in rentalForCreate.RentalItems)
            {
                var movie = movies.FirstOrDefault(m => m.Title == item.Title);
                //we must check that there is enough quantity to be rented in the database
                if ((movie == null) || (movie.NumberOfRentedItems >= movie.QuantityForRenting))
                {
                    ModelState.AddModelError("RentalItems", $"Error! Movie titled '{item.Title}' is not available for being rented from {rentalForCreate.RentalDateFrom.ToShortDateString()} to {rentalForCreate.RentalDateTo.ToShortDateString()}");
                }
                else
                {
                    // rental does not exist in the database yet and does not have a valid Id, so we must relate rentalitem to the object rental
                    rental.RentalItems.Add(new RentalItem(movie.Id, rental, movie.PriceForRenting, item.Description));
                    item.PriceForRenting = movie.PriceForRenting;
                }
            }
            rental.TotalPrice = rental.RentalItems.Sum(ri => ri.PriceForRenting * numDays);


            //if there is any problem because of the available quantity of movies or because the movie does not exist
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Add(rental);

            try
            {
                //we store in the database both rental and its rentalitems
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Rental", $"Error! There was an error while saving your rental, plese, try again later");
                return Conflict("Error" + ex.Message);

            }

            //it returns rentalDetail
            var rentalDetail = new RentalDetailDTO(rental.Id, rental.RentalDate,
                rental.CustomerUserName, rental.CustomerNameSurname,
                rental.DeliveryAddress, rentalForCreate.PaymentMethod,
                rental.RentalDateFrom, rental.RentalDateTo,
                rentalForCreate.RentalItems);

            return CreatedAtAction("GetRental", new { id = rental.Id }, rentalDetail);
        }
    }
}
