using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppForSEII2526.UT.BocadillosController_test
{
    public class GetBocadillosParaResenya_test : AppForSEII25264SqliteUT
    {
        public GetBocadillosParaResenya_test()
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

            _context.AddRange(tipopans);
            _context.AddRange(bocadillos);
            _context.SaveChanges();
        }

        public static IEnumerable<Object[]> TestCasesFor_GetBocadillosParaResenya_Ok()
        {
            var resenyasDTOs = new List<BocadilloDTO>() {
                new BocadilloDTO("Focata",Tamanyo.Normal,"sandwitch",10),
                new BocadilloDTO("Politecnico", Tamanyo.Normal,"barra",1000)
            };

            var resenyasDTOsTC1 = new List<BocadilloDTO>() { resenyasDTOs[0], resenyasDTOs[1] };

            var resenyasDTOsTC2 = new List<BocadilloDTO>() { resenyasDTOs[0] };

            var resenyasDTOsTC3 = new List<BocadilloDTO>() { resenyasDTOs[1] };

            var resenyasDTOsTC4 = new List<BocadilloDTO>() { };

            var allTest = new List<Object[]>()
            {
                new object[] { null, null, resenyasDTOsTC1, },
                new object[] { "Focata", null , resenyasDTOsTC2, },
                new object[] { null, (float)1000, resenyasDTOsTC3, },
                new object[] { "Completo", (float)1000, resenyasDTOsTC4, }
            };

            return allTest;
        }


        [Theory]
        [MemberData(nameof(TestCasesFor_GetBocadillosParaResenya_Ok))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetBocadillosParaResenya_Ok_test(string? nombre, float? PVP, IList<BocadilloDTO> bocadillos_esperados)
        {
            //Arrange
            var controller = new BocadillosController(_context, null);


            //Act

            var result = await controller.GetBocadillosParaResenya(nombre, PVP);


            //Assert

            var okresult = Assert.IsType<OkObjectResult>(result);

            var resenyaresult = Assert.IsType<List<BocadilloDTO>>(okresult.Value);
            Assert.Equal(bocadillos_esperados, resenyaresult);

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetBocadillosParaResenya_BadRequest_Test()
        {
            // Arrange
            var mock = new Mock<ILogger<BocadillosController>>();
            ILogger<BocadillosController> logger = mock.Object;
            var controller = new BocadillosController(_context, logger);

            // Act
            var result = await controller.GetBocadillosParaResenya("",(float)-10);

            //Assert
            //we check that the response type is OK and obtain the list of movies
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<string>(badRequestResult.Value);

            Assert.Equal("El PVP no puede ser negativo", problemDetails);
        }

    }
}
