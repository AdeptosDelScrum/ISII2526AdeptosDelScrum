using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.CompraDTOs;
using AppForSEII2526.API.Models;
using Humanizer.Localisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.ComprasController_test
{
    public class PostCompra_test: AppForSEII25264SqliteUT
    {
        private const string _nombre= "Paco";
        private const string _apellido1 = "Olivares";
        private const string _apellido2 = "Alonso";

        private const string _bocadillo1Nombre = "Completo";
        private const string _bocadillo1tipoPan = "Normal";
        private const string _bocadillo2Nombre = "Politecnico";
        private const string _bocadillo2tipoPan = "Integral";

        public PostCompra_test()
        {

            var tiposPan = new List<TipoPan>() {
                new TipoPan(1,_bocadillo1tipoPan),
                new TipoPan(2,_bocadillo2tipoPan),
            };

            var bocadillos = new List<Bocadillo>(){
                new Bocadillo(1,_bocadillo1Nombre,3,1,Tamanyo.Normal, tiposPan[0]),
                new Bocadillo(2,_bocadillo2Nombre, 2,1,Tamanyo.Pequenyo, tiposPan[1]),
            };

            ApplicationUser user = new ApplicationUser(1, "Paco", "Olivares", "Alonso");

            var compra = new Compra(_nombre,_apellido1, _apellido2, user, DateTime.Today, 1, new Tarjeta(),new List<CompraBocadillo>());
            compra.BocadillosComprados.Add(new CompraBocadillo(bocadillos[0], 1, compra));

            _context.ApplicationUser.Add(user);
            _context.AddRange(tiposPan);
            _context.AddRange(bocadillos);
            _context.Add(compra);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CrearCompra()
        {


            var compraNoItem = new CompraBocadilloForCreateDTO(_nombre, _apellido1, _apellido2, new Tarjeta(), new List<CompraBocadilloItemDTO>());
            

            var compraItems = new List<CompraBocadilloItemDTO>() { new CompraBocadilloItemDTO(_bocadillo1Nombre, 3, _bocadillo1tipoPan, 1) };
            var UserNoNombre = new CompraBocadilloForCreateDTO("", _apellido1, _apellido2, new Tarjeta(), compraItems);
            var CompraNoMetodoPago = new CompraBocadilloForCreateDTO(_nombre, _apellido1, _apellido2, new PagoNoSeleccionado(), compraItems);
            var CompraApplicationUser = new CompraBocadilloForCreateDTO("Luis", _apellido1, _apellido2, new Tarjeta(),compraItems);
            var CompraApplicationUserApellido = new CompraBocadilloForCreateDTO(_nombre, "", _apellido2, new Tarjeta(), compraItems);

            var compraItemNoDisponible = new CompraBocadilloForCreateDTO(_nombre, _apellido1, _apellido2, new Tarjeta(), 
                new List<CompraBocadilloItemDTO>() { new CompraBocadilloItemDTO(_bocadillo1Nombre, 3, _bocadillo1tipoPan, 2) });
            var compraItemCero = new CompraBocadilloForCreateDTO(_nombre, _apellido1, _apellido2, new Tarjeta(),
                new List<CompraBocadilloItemDTO>() { new CompraBocadilloItemDTO(_bocadillo1Nombre, 3, _bocadillo1tipoPan, 0) });
            var bocadilloNoExiste = new CompraBocadilloForCreateDTO(_nombre, _apellido1, _apellido2, new Tarjeta(),
                new List<CompraBocadilloItemDTO>() { new CompraBocadilloItemDTO("Americano", 3, _bocadillo1tipoPan, 1) });


            var allTests = new List<object[]>
            { 
                new object[] { compraNoItem, "Debe de seleccionar al menos un bocadillo para comprar",  },
                new object[] { UserNoNombre, "El cliente debe ingrsar su nombre",  },
                new object[] { CompraApplicationUser, "El usuario no está registrado", },
                new object[] { CompraApplicationUserApellido, "El cliente debe ingrsar su primer apellido", },
                new object[] { compraItemNoDisponible, $"No hay stock suficiente de '{_bocadillo1Nombre}'. Stock disponible: 1.", },
                new object[] { compraItemCero, $"Debe indicar una cantidad mayor que 0 para '{_bocadillo1Nombre}'.", },
                new object[] { bocadilloNoExiste, $"El bocadillo 'Americano' no existe.", },
                new object[] { CompraNoMetodoPago, "El cliente debe escoger un método de pago", },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CrearCompra))]
        public async Task CreateRental_Error_test(CompraBocadilloForCreateDTO compraDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;

            var controller = new ComprasController(_context, logger);

            // Act
            var result = await controller.CrearCompra(compraDTO);

            //Assert
            //we check that the response type is BadRequest and obtain the error returned
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            //we check that the expected error message and actual are the same
            Assert.StartsWith(errorExpected, errorActual);

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateRental_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;

            var controller = new ComprasController(_context, logger);

            DateTime to = DateTime.Today.AddDays(6);
            DateTime from = DateTime.Today.AddDays(7);

            var compraDTO = new CompraBocadilloDetailDTO(0,DateTime.Today,3,_nombre, _apellido1, _apellido2,1,new Tarjeta(),
                new List<CompraBocadilloItemDTO>()
                { new CompraBocadilloItemDTO(_bocadillo1Nombre, 3, _bocadillo1tipoPan, 1) });

            var expectedCompraDetailDTO = new CompraBocadilloDetailDTO(2, DateTime.Today, 3, _nombre, _apellido1, _apellido2, 1, new Tarjeta(),
                new List<CompraBocadilloItemDTO>()
                { new CompraBocadilloItemDTO(_bocadillo1Nombre, 3, _bocadillo1tipoPan, 1) });

            // Act
            var result = await controller.CrearCompra(compraDTO);

            //Assert
            //we check that the response type is BadRequest and obtain the error returned
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualCompraDetailDTO = Assert.IsType<CompraBocadilloDetailDTO>(createdResult.Value);

            Assert.Equal(expectedCompraDetailDTO, actualCompraDetailDTO);

        }

    }
}
