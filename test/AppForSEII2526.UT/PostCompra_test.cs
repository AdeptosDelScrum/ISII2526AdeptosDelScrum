using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using Xunit;
using Moq;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using AppForSEII2526.UT;                      // Base AppForSEII25264SqliteUT
using AppForSEII2526.API.Controllers;        // BonosBocadilloController
using AppForSEII2526.API.DTOs;               // CrearCompraDTO, CrearCompraItemDTO, CompraDetailsDTO
using AppForSEII2526.Models;                 // BonoBocadillo, TipoBocadillo, MetodoPago

namespace AppForSEII2526.UT.BonosBocadilloController_test
{
    public class PostCompra_test : AppForSEII25264SqliteUT
    {
        public PostCompra_test()
        {
            // ----- Semilla mínima de datos para los tests -----
            // Tipos
            var tNormal = new TipoBocadillo { IdTipo = 1, NombreTipo = "normal" };
            var tVegano = new TipoBocadillo { IdTipo = 2, NombreTipo = "vegano" };

            // Bonos con stock (PVP en la entidad)
            var bono1 = new BonoBocadillo
            {
                BonoId = 1,
                Nombre = "Bono Mixto 10",
                NBocadillos = 10,
                CantidadDisponible = 50,
                PVP = 3.50m,
                IdTipo = 1,
                TipoBocadillo = tNormal
            };

            var bono2 = new BonoBocadillo
            {
                BonoId = 2,
                Nombre = "Bono Vegano 5",
                NBocadillos = 5,
                CantidadDisponible = 2,
                PVP = 4.00m,
                IdTipo = 2,
                TipoBocadillo = tVegano
            };

            // Metodo de pago (la impl del controller espera _context.MetodosPago con propiedad Id)
            var mp = new MetodoPago { };
            TrySetSilent(mp, "Id", 1);
            TrySetSilent(mp, "Nombre", "Tarjeta"); // por si tu Compra guarda string de nombre

            _context.AddRange(tNormal, tVegano, bono1, bono2, mp);
            _context.SaveChanges();
        }

        // -------------------------
        // Casos de BadRequest (400)
        // -------------------------
        public static IEnumerable<object[]> TestCases_BadRequest()
        {
            yield return new object[]
            {
                // Sin items -> "Debe incluir al menos un bono."
                new CrearCompraDTO
                {
                    NombreCompleto = "Ana",
                    Apellidos = "Diaz",
                    MetodoPagoId = 1L,
                    Items = new List<CrearCompraItemDTO>() // vacío
                },
                "Debe incluir al menos un bono."
            };

            yield return new object[]
            {
                // Metodo de pago inexistente -> "Metodo de pago invalido."
                new CrearCompraDTO
                {
                    NombreCompleto = "Ana",
                    Apellidos = "Diaz",
                    MetodoPagoId = 999L,
                    Items = new List<CrearCompraItemDTO>
                    {
                        new CrearCompraItemDTO { BonoId = 1, Cantidad = 1 }
                    }
                },
                "Metodo de pago invalido."
            };

            yield return new object[]
            {
                // Bono inexistente -> "Bono(s) inexistente(s):"
                new CrearCompraDTO
                {
                    NombreCompleto = "Ana",
                    Apellidos = "Diaz",
                    MetodoPagoId = 1L,
                    Items = new List<CrearCompraItemDTO>
                    {
                        new CrearCompraItemDTO { BonoId = 123, Cantidad = 1 }
                    }
                },
                "Bono(s) inexistente(s):"
            };

            yield return new object[]
            {
                // Cantidad invalida (0) -> "Cantidad invalida para bono {id}."
                new CrearCompraDTO
                {
                    NombreCompleto = "Ana",
                    Apellidos = "Diaz",
                    MetodoPagoId = 1L,
                    Items = new List<CrearCompraItemDTO>
                    {
                        new CrearCompraItemDTO { BonoId = 1, Cantidad = 0 }
                    }
                },
                "Cantidad invalida para bono"
            };
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCases_BadRequest))]
        public async Task PostCompra_BadRequest_Test(CrearCompraDTO dto, string errorExpectedStartsWith)
        {
            // Arrange
            var mock = new Mock<ILogger<BonosBocadilloController>>();
            var logger = mock.Object;
            var controller = new BonosBocadilloController(_context, logger);

            // Act
            var result = await controller.PostCompra(dto, CancellationToken.None);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var msg = Assert.IsType<string>(bad.Value);
            Assert.StartsWith(errorExpectedStartsWith, msg);
        }

        // --------------------------
        // Caso de Conflict (409)
        // --------------------------
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task PostCompra_Conflict_StockInsuficiente()
        {
            // Arrange
            var controller = new BonosBocadilloController(_context, new Mock<ILogger<BonosBocadilloController>>().Object);

            // BonoId=2 tiene CantidadDisponible = 2 (semilla). Pedimos 3.
            var dto = new CrearCompraDTO
            {
                NombreCompleto = "Carlos",
                Apellidos = "Perez",
                MetodoPagoId = 1L,
                Items = new List<CrearCompraItemDTO>
                {
                    new CrearCompraItemDTO { BonoId = 2, Cantidad = 3 }
                }
            };

            // Act
            var result = await controller.PostCompra(dto, CancellationToken.None);

            // Assert
            var conflict = Assert.IsType<ConflictObjectResult>(result);
            var msg = Assert.IsType<string>(conflict.Value);
            Assert.Contains("Stock insuficiente", msg);
        }

        // --------------------------
        // Caso OK (201 Created)
        // --------------------------
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task PostCompra_Ok_CreaCompra_DisminuyeStock()
        {
            // Arrange
            var controller = new BonosBocadilloController(_context, new Mock<ILogger<BonosBocadilloController>>().Object);

            var dto = new CrearCompraDTO
            {
                NombreCompleto = "Juan",
                Apellidos = "Lopez",
                MetodoPagoId = 1L,
                Items = new List<CrearCompraItemDTO>
                {
                    new CrearCompraItemDTO { BonoId = 1, Cantidad = 2 }, // 2 * 3.50 = 7.00
                    new CrearCompraItemDTO { BonoId = 2, Cantidad = 1 }  // 1 * 4.00 = 4.00
                }
            };

            // Act
            var result = await controller.PostCompra(dto, CancellationToken.None);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            var details = Assert.IsType<CompraDetailsDTO>(created.Value);

            Assert.Equal("Juan", details.NombreCompleto);
            Assert.Equal("Lopez", details.Apellidos);
            Assert.Equal(2, details.Items.Count);

            var totalEsperado = (3.50m * 2) + (4.00m * 1);
            Assert.Equal(totalEsperado, details.PrecioTotal);

            // Stock actualizado
            var b1 = await _context.Set<BonoBocadillo>().SingleAsync(b => b.BonoId == 1);
            var b2 = await _context.Set<BonoBocadillo>().SingleAsync(b => b.BonoId == 2);
            Assert.Equal(48, b1.CantidadDisponible); // 50 - 2
            Assert.Equal(1,  b2.CantidadDisponible); //  2 - 1
        }

        // -------- Helpers (reflexion segura) --------
        private static bool TrySetSilent(object entity, string propName, object? value)
        {
            var p = entity.GetType().GetProperty(propName);
            if (p == null || !p.CanWrite) return false;
            try
            {
                var targetType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                var converted = value == null ? null : Convert.ChangeType(value, targetType);
                p.SetValue(entity, converted);
                return true;
            }
            catch { return false; }
        }
    }
}
