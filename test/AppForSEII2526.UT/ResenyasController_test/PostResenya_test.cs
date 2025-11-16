using AppForSEII2526.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.ResenyasController_test
{
    public class PostResenya_test : AppForSEII25264SqliteUT
    {
        public PostResenya_test()
        {
            var tipopan0 = new TipoPan();
            tipopan0.PanId = 1;
            tipopan0.Nombre = "sandwitch";
            var bocadillo0 = new Bocadillo();
            bocadillo0.Id = 1;
            bocadillo0.Nombre = "Focata";
            bocadillo0.PVP = 10;
            bocadillo0.Stock = 3;
            bocadillo0.TamanyoBocadillo = Tamanyo.Normal;
            bocadillo0.TipoPan = tipopan0;


            var tipopan1 = new TipoPan();
            tipopan1.PanId = 2;
            tipopan1.Nombre = "barra";
            var bocadillo1 = new Bocadillo();
            bocadillo1.Id = 2;
            bocadillo1.Nombre = "Politecnico";
            bocadillo1.PVP = 1000;
            bocadillo1.Stock = 100;
            bocadillo1.TamanyoBocadillo = Tamanyo.Normal;
            bocadillo1.TipoPan = tipopan1;

            var bocadillos = new List<Bocadillo>();
            bocadillos.Add(bocadillo1);
            bocadillos.Add(bocadillo0);
            var tipopans = new List<TipoPan>();
            tipopans.Add(tipopan1);
            tipopans.Add(tipopan0);

            ApplicationUser applicationUser = new ApplicationUser(1,"Paco", "Salazar", "Mendoza");
            _context.Add(applicationUser);
            _context.AddRange(tipopans);
            _context.AddRange(bocadillos);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_PostResenya()
        {
            var tipopan0 = new TipoPan();
            tipopan0.PanId = 1;
            tipopan0.Nombre = "sandwitch";
            var bocadillo0 = new BocadilloDTO("Focata",Tamanyo.Normal,"sandwitch",10);
            bocadillo0.Id = 1;
            var bocadillos = new List<LineasResenyaDTO>();
            bocadillos.Add(new LineasResenyaDTO(bocadillo0, 3));


            var resenyaSinTíitulo= new ResenyaDTO(1,"Pedro","","Descripcion",3,bocadillos, "Paco", "Salazar", "Mendoza");

            var resenyaSinDescripcion = new ResenyaDTO(1, "Pedro", "Titulo", "", 3, bocadillos, "Paco", "Salazar", "Mendoza");

            var resenyaSinBocadillos = new ResenyaDTO(1, "Pedro", "Titulo", "Descripcion", 3, new List<LineasResenyaDTO>(), "Paco", "Salazar", "Mendoza");

            var resenyaRateNoValido = new ResenyaDTO(1, "Pedro", "Titulo", "Descripcion", -10, bocadillos, "Paco", "Salazar", "Mendoza");

            var bocadillo1= new BocadilloDTO("Focata", Tamanyo.Normal, "sandwitch", 10);
            bocadillo1.Id = 100;
            var bocadillos0 = new List<LineasResenyaDTO>();
            bocadillos0.Add(new LineasResenyaDTO(bocadillo1, 3));

            var resenyaBocadilloNoExiste = new ResenyaDTO(1, "Pedro", "Titulo", "Descripcion", 3, bocadillos0, "Paco", "Salazar", "Mendoza");

            var bocadillos1 = new List<LineasResenyaDTO>();
            bocadillos1.Add(new LineasResenyaDTO(bocadillo0, -10));

            var resenyaPuntBocadilloNoValida = new ResenyaDTO(1, "Pedro", "Titulo", "Descripcion", 3, bocadillos1, "Paco", "Salazar", "Mendoza");

            var resenyaNombreClienteNoExiste = new ResenyaDTO(1, "Pedro", "Titulo", "Descripcion", 3, bocadillos, "", "Salazar", "Mendoza");

            var allTests = new List<object[]>
            {             //input for createpurchase - Error expected
                new object[] { resenyaSinTíitulo, "La reseña tiene que tener un título",  },
                new object[] { resenyaSinDescripcion, "El campo descripción está vacío", },
                new object[] { resenyaSinBocadillos, "No se puede crear una reseña sin bocadillos", },
                new object[] { resenyaRateNoValido, "La valoracuón general tiene que ser entre 1 y 5 estrellas", },
                new object[] { resenyaBocadilloNoExiste, "Uno de los bocadillos introducidos para reseñar no existe", },
                new object[] { resenyaPuntBocadilloNoValida, "La puntuación debe ser un valor numérico entre 0 y 10", },
                new object[] { resenyaNombreClienteNoExiste, "Tienes que iniciar sesión para hacer una reseña", },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_PostResenya))]
        public async Task CreateRental_Error_test(ResenyaDTO resenyaDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<ResenyasController>>();
            ILogger<ResenyasController> logger = mock.Object;

            var controller = new ResenyasController(_context, logger);

            // Act
            var result = await controller.PostResenya(resenyaDTO);

            //Assert
            //we check that the response type is BadRequest and obtain the error returned
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<String>(badRequestResult.Value);

            var errorActual = problemDetails;

            //we check that the expected error message and actual are the same
            Assert.StartsWith(errorExpected, errorActual);

        }


        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task PostResenya_Ok_Test()
        {
            // Arrange
            var mock = new Mock<ILogger<ResenyasController>>();
            ILogger<ResenyasController> logger = mock.Object;
            var controller = new ResenyasController(_context, logger);
            List<LineasResenyaDTO> lineasResenyaDTOs = new List<LineasResenyaDTO>();
            var bocadillo = new BocadilloDTO("Focata", Tamanyo.Normal, "sandwitch", (float)10);
            bocadillo.Id = 1;
            lineasResenyaDTOs.Add(new LineasResenyaDTO(bocadillo,3));


            var detailsLineasResenyaDTO = new List<LineasResenyaDTO>();
            detailsLineasResenyaDTO.Add(new LineasResenyaDTO(bocadillo,3));
            var resenyaDetailsExpected = new DetailsResenyaDTO(1 ,"Paco","resenya1","Me ha gustado",DateTime.Today,3,detailsLineasResenyaDTO, "Paco", "Salazar", "Mendoza");

            // Act
            var result = await controller.PostResenya(new ResenyaDTO(3,"Paco","resenya1","Me ha gustado",3,lineasResenyaDTOs, "Paco", "Salazar", "Mendoza"));

            //Assert
            //we check that the response type is OK and obtain the list of movies
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var resenyaDetails = Assert.IsType<DetailsResenyaDTO>(createdResult.Value);

            Assert.Equal(resenyaDetailsExpected, resenyaDetails);
        }
    }
}
