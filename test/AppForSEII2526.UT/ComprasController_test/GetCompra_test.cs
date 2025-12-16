using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.CompraDTOs;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.ComprasController_test
{
    public class GetCompra_test: AppForSEII25264SqliteUT
    {
        private readonly DateTime _fechaCompra;
        public GetCompra_test()
        {
            var fechaCompra = DateTime.Today;
            var tiposPan = new List<TipoPan>() {
                new TipoPan(1, "Normal"),
                new TipoPan(2, "Semillas"),
                new TipoPan(3, "Integral")
            };

            var bocadillos = new List<Bocadillo>(){
                 new Bocadillo(1, "Completo",3, 50, Tamanyo.Normal, tiposPan[0]),
                new Bocadillo(2, "Politecnico", 4, 70, Tamanyo.Pequenyo, tiposPan[0]),
                new Bocadillo(3, "Americano", (float)2.5, 30, Tamanyo.Normal, tiposPan[2]),
            };

            ApplicationUser user = new ApplicationUser(1, "Paco", "Olivares", "Alonso");
            var tarjeta = new Tarjeta();
            _context.MetodoPago.Add(tarjeta);
            var compra = new Compra("Paco", "Olivares", "Alonso", user, _fechaCompra, 1, tarjeta, new List<CompraBocadillo>());

            var compraBocadillo = new CompraBocadillo(bocadillos[0], 1, compra);

            compra.BocadillosComprados.Add(compraBocadillo);

            _context.ApplicationUser.Add(user);
            _context.AddRange(tiposPan);
            _context.AddRange(bocadillos);
            _context.Add(compra);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompra_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;

            var controller = new ComprasController(_context, logger);

            // Act
            var result = await controller.GetCompra(0);

            //Assert
            Assert.IsType<NotFoundResult>(result);

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetCompra_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;
            var controller = new ComprasController(_context, logger);


            var compraEsperada = new CompraBocadilloDetailDTO(1, _fechaCompra, 3,"Paco", "Olivares", "Alonso",
                        0, 1,
                        new List<CompraBocadilloItemDTO>());
            compraEsperada.BocadillosComprados.Add(new CompraBocadilloItemDTO("Completo", 3, "Normal", 1));

            // Act 
            var result = await controller.GetCompra(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var compraDTOActual = Assert.IsType<CompraBocadilloDetailDTO>(okResult.Value);

            var eq = compraEsperada.Equals(compraDTOActual);
            
            Assert.Equal(compraEsperada, compraDTOActual);

        }
    }
}
