using AppForSEII2526.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResenyasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ResenyasController> _logger;

        public ResenyasController(ApplicationDbContext context, ILogger<ResenyasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(DetailsResenyaDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> PostResenya(ResenyaDTO resenya)
        {

            if (resenya.Title.IsNullOrEmpty())
            {
                return BadRequest("La reseña tiene que tener un título");
            }
            if (resenya.Description.IsNullOrEmpty())
            {
                return BadRequest("El campo descripción está vacío");
            }
            if (resenya.Lineas.Count == 0)
            {
                return BadRequest("No se puede crear una reseña sin bocadillos");
            }
            if(resenya.Rate < 0 || resenya.Rate > 5)
            {
                return BadRequest("La valoracuón general tiene que ser entre 1 y 5 estrellas");
            }

            var user = _context.ApplicationUser.FirstOrDefault(au => au.NombreCliente == resenya.Nombre_cliente);
            if (user == null)
                return BadRequest("Tienes que iniciar sesión para hacer una reseña");

            var resenyaObj = new Resenya(user.NombreCliente, user.Apellido1_Cliente,user.Apellido2_Cliente, user);
            resenyaObj.descripcion = resenya.Description;
            resenyaObj.fechaPublicacion = DateTime.Today;
            resenyaObj.nombreUsuario = resenya.Name;
            resenyaObj.titulo = resenya.Title;
            switch (resenya.Rate)
            {
                case 1:
                    resenyaObj.valoracion = Resenya.rate.Una;
                    break;
                case 2:
                    resenyaObj.valoracion = Resenya.rate.Dos;
                    break;
                case 3:
                    resenyaObj.valoracion = Resenya.rate.Tres;
                    break;
                case 4:
                    resenyaObj.valoracion = Resenya.rate.Cuatro;
                    break;
                case 5:
                    resenyaObj.valoracion = Resenya.rate.Cinco;
                    break;
            }
            

            var bocadillos = _context.Bocadillo.ToList();
            List<ResenyaBocadillo> lineasResenyaObj = new List<ResenyaBocadillo>();
            List<DetailsLineasResenyaDTO> detailsLineasDTOs = new List<DetailsLineasResenyaDTO>();
            foreach (var linea in resenya.Lineas)
            {
                var bocadillo = bocadillos.FirstOrDefault(b => b.Nombre==linea.bocadillo.Name);
                if (bocadillo == null) {
                    return BadRequest("Uno de los bocadillos introducidos para reseñar no existe");
                }
                if (linea.Puntuacion > 10 || linea.Puntuacion < 0) {
                    return BadRequest("La puntuación debe ser un valor numérico entre 0 y 10");
                }
                var lineaResenyaObj = new ResenyaBocadillo();
                var detailsLineasDTO = new DetailsLineasResenyaDTO();

                lineaResenyaObj.Resenya = resenyaObj;
                lineaResenyaObj.Bocadillo = bocadillo;
                lineaResenyaObj.Puntuacion = linea.Puntuacion;
                lineasResenyaObj.Add(lineaResenyaObj);

                detailsLineasDTO.nombre = lineaResenyaObj.Bocadillo.Nombre;
                detailsLineasDTO.precio = lineaResenyaObj.Bocadillo.PVP;
                detailsLineasDTO.tamanyo = lineaResenyaObj.Bocadillo.TamanyoBocadillo;
                detailsLineasDTO.puntuacion = lineaResenyaObj.Puntuacion;
                detailsLineasDTOs.Add(detailsLineasDTO);
            }

            resenyaObj.ResenyaBocadillo = lineasResenyaObj;

            _context.Add(resenyaObj);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Reseña", $"Error! There was an error while saving your reseña, plese, try again later");
                return Conflict("Error" + ex.Message);

            }

            var detailresenya = 
                new DetailsResenyaDTO(resenyaObj.Id,resenyaObj.nombreUsuario, resenyaObj.titulo, resenyaObj.descripcion, resenyaObj.fechaPublicacion, 
                    (int)resenyaObj.valoracion + 1, (List<LineasResenyaDTO>)resenya.Lineas, resenyaObj.User.NombreCliente,resenyaObj.User.Apellido1_Cliente,resenyaObj.User.Apellido2_Cliente);


            return CreatedAtAction("GetResenya", new { id = resenyaObj.Id }, detailresenya);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(DetailsResenyaDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetResenya(int id) 
        {
            if(_context.Resenyas == null)
            {
                _logger.LogError("Error: la tabla resenya no existe");
                return NotFound();
            }

            var resenya = await _context.Resenyas
                .Where(r => r.Id == id)
                .Include(r => r.ResenyaBocadillo)
                    .ThenInclude(r => r.Bocadillo)
                .Include(r => r.User)
                .Select(r => new DetailsResenyaDTO(r.Id,r.nombreUsuario,r.titulo,r.descripcion,r.fechaPublicacion,((int)r.valoracion + 1),
                            r.ResenyaBocadillo.Select( lr => new LineasResenyaDTO(new BocadilloDTO(lr.Bocadillo.Nombre,lr.Bocadillo.TamanyoBocadillo,lr.Bocadillo.TipoPan.Nombre,lr.Bocadillo.PVP),lr.Puntuacion)).ToList(), r.User.NombreCliente, r.User.Apellido1_Cliente, r.User.Apellido2_Cliente))
                .FirstOrDefaultAsync();
            
            if(resenya == null)
            {
                _logger.LogError("Error: la reseña no existe");
                return NotFound();
            }

            return Ok(resenya);
                
        }

    }
}
