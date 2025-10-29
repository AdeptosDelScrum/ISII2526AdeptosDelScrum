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
        private readonly ILogger<BocadillosController> _logger;

        public ResenyasController(ApplicationDbContext context, ILogger<BocadillosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
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
            if (resenya.Rate == null)
            {
                return BadRequest("Tienes que aportar una valoración general");
            }
            if (resenya.Lineas.Count == 0)
            {
                return BadRequest("No se puede crear una reseña sin bocadillos");
            }
            if(resenya.Rate < 0 || resenya.Rate > 5)
            {
                return BadRequest("La valoracuón general tiene que ser entre 1 y 5 estrellas");
            }

            var resenyaObj = new Resenya();
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
            foreach (var linea in resenya.Lineas)
            {
                var bocadillo = bocadillos.FirstOrDefault(b => b.Id==linea.bocadillo.Id);
                if (bocadillo == null) {
                    return BadRequest("Uno de los bocadillos introducidos para reseñar no existe");
                }
                if (linea.Puntuacion > 10 || linea.Puntuacion < 0) {
                    return BadRequest("La puntuación debe ser un valor numérico entre 0 y 10");
                }
                var lineaResenyaObj = new ResenyaBocadillo();
                lineaResenyaObj.Resenya = resenyaObj;
                lineaResenyaObj.Bocadillo = bocadillo;
                lineaResenyaObj.Puntuacion = linea.Puntuacion;
                lineasResenyaObj.Add(lineaResenyaObj);
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

            return Ok("Ole");



        }
    }
}
