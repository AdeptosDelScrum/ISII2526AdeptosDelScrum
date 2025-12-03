using AppForSEII2526.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.ResenyasController_test
{
    public class GetResenya_test : AppForSEII25264SqliteUT
    {

        public GetResenya_test()
        {

            var tipopan0 = new TipoPan();
            tipopan0.PanId = 100;
            tipopan0.Nombre = "sandwitch";
            var bocadillo0 = new Bocadillo();
            bocadillo0.Id = 101;
            bocadillo0.Nombre = "Focata";
            bocadillo0.PVP = 10;
            bocadillo0.Stock = 3;
            bocadillo0.TamanyoBocadillo = Tamanyo.Normal;
            bocadillo0.TipoPan = tipopan0;


            ApplicationUser applicationUser = new ApplicationUser(1, "Paco", "Salazar", "Mendoza");
            var resenya = new Resenya("Paco", "Salazar", "Mendoza", applicationUser);
            resenya.Id = 102;
            resenya.descripcion = "Me ha gustado";
            resenya.fechaPublicacion = DateTime.Parse("2025-11-12T00:00:00.0000000");
            resenya.nombreUsuario = "Pepe";
            resenya.titulo = "Resenya1";
            resenya.valoracion = Resenya.rate.Cinco;
            resenya.ResenyaBocadillo = new List<ResenyaBocadillo>();
            var linea = new ResenyaBocadillo();
            linea.BocadilloId = 101;
            linea.ResenyaId = 102;
            linea.Puntuacion = 3;
            resenya.ResenyaBocadillo.Add(linea);

            _context.Add(applicationUser);   
            _context.Add(tipopan0);
            _context.Add(bocadillo0);
            _context.Add(linea);
            _context.Add(resenya);
            _context.SaveChanges();
        }


        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetResenya_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ResenyasController>>();
            ILogger<ResenyasController> logger = mock.Object;

            var controller = new ResenyasController(_context, logger);

            // Act
            var result = await controller.GetResenya(0);

            //Assert
            //we check that the response type is OK and obtain the list of movies
            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetResenya_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ResenyasController>>();
            ILogger<ResenyasController> logger = mock.Object;
            var controller = new ResenyasController(_context, logger);



            var expectedBocadillos = new List<LineasResenyaDTO>();
            expectedBocadillos.Add(new LineasResenyaDTO(new BocadilloDTO("Focata", Tamanyo.Normal, "sandwitch",10),3));

            var expectedResenya = new DetailsResenyaDTO(0,"Pepe","Resenya1","Me ha gustado", DateTime.Parse("2025-11-12T00:00:00.0000000"),5,expectedBocadillos, "Paco", "Salazar", "Mendoza");

            // Act 
            var result = await controller.GetResenya(102);

            //Assert
            //we check that the response type is OK and obtain the rental
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resenyaDTOActual = Assert.IsType<DetailsResenyaDTO>(okResult.Value);
            var eq = expectedResenya.Equals(resenyaDTOActual);
            //we check that the expected and actual are the same
            Assert.Equal(expectedResenya, resenyaDTOActual);

        }
    }
}
