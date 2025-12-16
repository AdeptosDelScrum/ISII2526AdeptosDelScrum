using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;

namespace AppForSEII2526.UT.BocadillosController_test
{
    public class GetBocadillosParaCompra_test: AppForSEII25264SqliteUT
    {
        public GetBocadillosParaCompra_test()
        {
            var tiposPan = new List<TipoPan>()
            {
                new TipoPan(1, "Normal"),
                new TipoPan(2, "Semillas"),
                new TipoPan(3, "Integral")
            };

            var bocadillos = new List<Bocadillo>()
            {
                new Bocadillo(1, "Completo",3, 50, Tamanyo.Normal, tiposPan[0]),
                new Bocadillo(2, "Politecnico", 4, 70, Tamanyo.Pequenyo, tiposPan[0]),
                new Bocadillo(3, "Americano", (float)2.5, 30, Tamanyo.Normal, tiposPan[2])
            };

            ApplicationUser user = new ApplicationUser(1,"Paco", "Olivares", "Alonso");

            var compra = new Compra("Paco", "Olivares", "Alonso", user,DateTime.Today, 1, new Tarjeta(), new List<CompraBocadillo>());

            var compraBocadillo = new CompraBocadillo(bocadillos[0], 1, compra);

            _context.Add(user);
            _context.AddRange(tiposPan);
            _context.AddRange(bocadillos);
            _context.Add(compra);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetBocadillosParaCompra_OK()
        {
            var bocadillosDTOs = new List<BocadilloDTO>()
            {
                new BocadilloDTO("Completo", Tamanyo.Normal, "Normal", 3),
                new BocadilloDTO("Americano", Tamanyo.Normal, "Integral", (float)2.5),
                new BocadilloDTO("Politecnico", Tamanyo.Pequenyo, "Normal", 4)
            };

            var bocadilloDTOs_TC1 = new List<BocadilloDTO>() { bocadillosDTOs[0], bocadillosDTOs[1], bocadillosDTOs[2] }
                .OrderBy(b => b.Name).ToList();

            var bocadilloDTOs_TC2 = new List<BocadilloDTO>() { bocadillosDTOs[0], bocadillosDTOs[1] }
                .OrderBy(b => b.Name).ToList();
            var bocadilloDTOs_TC3 = new List<BocadilloDTO>() { bocadillosDTOs[1] };

            var bocadilloDTOs_TC4 = new List<BocadilloDTO>() {bocadillosDTOs[2] };

            var allTests = new List<object[]>
            {             
                new object[] { null, null, bocadilloDTOs_TC1,  },
                new object[] {Tamanyo.Normal, null, bocadilloDTOs_TC2, },
                new object[] { null, "Integral", bocadilloDTOs_TC3, },
                new object[] {Tamanyo.Pequenyo, "Normal", bocadilloDTOs_TC4, },
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetBocadillosParaCompra_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetBocadillosParaCompra_OK_test(Tamanyo? tamanyo,string? tipoPan, IList<BocadilloDTO> bocadillos_esperados)
        {
            // Arrange
            var controller = new BocadillosController(_context, null);

            // Act
            var result = await controller.GetBocadillosParaCompra(tamanyo, tipoPan);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            var bocadillosActual = Assert.IsType<List<BocadilloDTO>>(okResult.Value);
            Assert.Equal(bocadillos_esperados, bocadillosActual);

        }


        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetBocadillosParaCompra_badrequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<BocadillosController>>();
            ILogger<BocadillosController> logger = mock.Object;
            var controller = new BocadillosController(_context, logger);

            // Act
            var result = await controller.GetBocadillosParaCompra(null, "Cosas");

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            var problem = problemDetails.Errors.First().Value[0];

            Assert.Equal("No se ha encontrado ese tipo de pan", problem);
        }
    }
}
