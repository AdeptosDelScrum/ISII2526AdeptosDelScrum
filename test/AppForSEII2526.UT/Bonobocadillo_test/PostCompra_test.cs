using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using AppForSEII2526.UT;                      // AppForSEII25264SqliteUT
using AppForSEII2526.API.DTOs;               // CrearCompraDTO, CrearCompraItemDTO, CompraDetailsDTO
using AppForSEII2526.Models;                 // BonoBocadillo, TipoBocadillo, MetodoPago

namespace AppForSEII2526.UT.Bonobocadillo_test
{
    public class PostCompra_test : AppForSEII25264SqliteUT
    {
        public PostCompra_test()
        {
            // ----- Semilla minima -----
            var tNormal = new TipoBocadillo { IdTipo = 1, NombreTipo = "normal" };
            var tVegano = new TipoBocadillo { IdTipo = 2, NombreTipo = "vegano" };

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

            _context.AddRange(tNormal, tVegano, bono1, bono2);

            // Siembra MetodoPago buscando el tipo concreto mapeado por EF
            SeedMetodoPagoConcreto(id: 1L, nombre: "Tarjeta");

            _context.SaveChanges();
        }

        // ======================= Helpers de siembra / reflexion =======================

        /// Busca en el modelo EF un tipo concreto asignable a MetodoPago, lo instancia incluso sin ctor publico,
        /// y rellena Id/Nombre (o alias MetodoPagoId/Descripcion) por refleccion.
        private void SeedMetodoPagoConcreto(long id, string nombre)
        {
            var entityTypeClr = _context.Model
                .GetEntityTypes()
                .Select(et => et.ClrType)
                .FirstOrDefault(t => typeof(MetodoPago).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            if (entityTypeClr == null)
            {
                // No hay entidad MetodoPago en el modelo -> los tests de BadRequest por metodo pago inexistente seguiran pasando.
                return;
            }

            object mp;

            // Intentar ctor vacio publico o no publico; si no, usar objeto sin inicializar
            var ctor = entityTypeClr.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                                                    binder: null, types: Type.EmptyTypes, modifiers: null);
            if (ctor != null)
                mp = ctor.Invoke(null);
            else
                mp = FormatterServices.GetUninitializedObject(entityTypeClr);

            // Id (o MetodoPagoId)
            if (!TrySet(mp, "Id", id))
                TrySet(mp, "MetodoPagoId", id);

            // Nombre (o Descripcion)
            if (!TrySet(mp, "Nombre", nombre))
                TrySet(mp, "Descripcion", nombre);

            _context.Add(mp);
        }

        /// Setter silencioso y tolerante a nulos/convertibles
        private static bool TrySet(object obj, string prop, object? value)
        {
            var p = obj.GetType().GetProperty(prop, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (p == null || !p.CanWrite) return false;
            try
            {
                var target = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                var converted = value == null ? null : Convert.ChangeType(value, target);
                p.SetValue(obj, converted);
                return true;
            }
            catch { return false; }
        }

        /// Localiza el controller en singular o plural y lo instancia con (DbContext, ILogger) -> pasamos null al logger.
        private dynamic CreateController()
        {
            var apiAsm = typeof(CrearCompraDTO).Assembly.GetName().Name; // AppForSEII2526.API
            var t = Type.GetType($"AppForSEII2526.API.Controllers.BonosBocadilloController, {apiAsm}")
                    ?? Type.GetType($"AppForSEII2526.API.Controllers.BonoBocadilloController, {apiAsm}");
            if (t == null)
                throw new InvalidOperationException("No se encontro el controlador BonosBocadilloController/BonoBocadilloController.");

            var instance = Activator.CreateInstance(t, _context, null);
            if (instance == null)
                throw new InvalidOperationException("No se pudo instanciar el controlador.");
            return instance;
        }

        // =================================== TESTS ===================================

        // Casos de BadRequest (400)
        public static IEnumerable<object[]> TestCases_BadRequest()
        {
            yield return new object[]
            {
                new CrearCompraDTO
                {
                    NombreCompleto = "Ana",
                    Apellidos = "Diaz",
                    MetodoPagoId = 1L,
                    Items = new List<CrearCompraItemDTO>() // vacio
                },
                "Debe incluir al menos un bono."
            };

            yield return new object[]
            {
                new CrearCompraDTO
                {
                    NombreCompleto = "Ana",
                    Apellidos = "Diaz",
                    MetodoPagoId = 999L, // inexistente
                    Items = new List<CrearCompraItemDTO>
                    {
                        new CrearCompraItemDTO { BonoId = 1, Cantidad = 1 }
                    }
                },
                "Metodo de pago invalido."
            };

            yield return new object[]
            {
                new CrearCompraDTO
                {
                    NombreCompleto = "Ana",
                    Apellidos = "Diaz",
                    MetodoPagoId = 1L,
                    Items = new List<CrearCompraItemDTO>
                    {
                        new CrearCompraItemDTO { BonoId = 123, Cantidad = 1 } // bono inexistente
                    }
                },
                "Bono(s) inexistente(s):"
            };

            yield return new object[]
            {
                new CrearCompraDTO
                {
                    NombreCompleto = "Ana",
                    Apellidos = "Diaz",
                    MetodoPagoId = 1L,
                    Items = new List<CrearCompraItemDTO>
                    {
                        new CrearCompraItemDTO { BonoId = 1, Cantidad = 0 } // cantidad invalida
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
            dynamic controller = CreateController();

            var result = await controller.PostCompra(dto, CancellationToken.None);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            var msg = Assert.IsType<string>(bad.Value);
            Assert.StartsWith(errorExpectedStartsWith, msg);
        }

        // Conflict (409) por stock insuficiente
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task PostCompra_Conflict_StockInsuficiente()
        {
            dynamic controller = CreateController();

            var dto = new CrearCompraDTO
            {
                NombreCompleto = "Carlos",
                Apellidos = "Perez",
                MetodoPagoId = 1L,
                Items = new List<CrearCompraItemDTO>
                {
                    new CrearCompraItemDTO { BonoId = 2, Cantidad = 3 } // stock = 2
                }
            };

            var result = await controller.PostCompra(dto, CancellationToken.None);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            var msg = Assert.IsType<string>(conflict.Value);
            Assert.Contains("Stock insuficiente", msg);
        }

        // OK (201) crea compra y actualiza stock
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task PostCompra_Ok_CreaCompra_DisminuyeStock()
        {
            dynamic controller = CreateController();

            var dto = new CrearCompraDTO
            {
                NombreCompleto = "Juan",
                Apellidos = "Lopez",
                MetodoPagoId = 1L,
                Items = new List<CrearCompraItemDTO>
                {
                    new CrearCompraItemDTO { BonoId = 1, Cantidad = 2 }, // 7.00
                    new CrearCompraItemDTO { BonoId = 2, Cantidad = 1 }  // 4.00
                }
            };

            var result = await controller.PostCompra(dto, CancellationToken.None);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            var details = Assert.IsType<CompraDetailsDTO>(created.Value);

            Assert.Equal("Juan", details.NombreCompleto);
            Assert.Equal("Lopez", details.Apellidos);
            Assert.Equal(2, details.Items.Count);

            var totalEsperado = (3.50m * 2) + (4.00m * 1);
            Assert.Equal(totalEsperado, details.PrecioTotal);

            var b1 = await _context.Set<BonoBocadillo>().SingleAsync(b => b.BonoId == 1);
            var b2 = await _context.Set<BonoBocadillo>().SingleAsync(b => b.BonoId == 2);
            Assert.Equal(48, b1.CantidadDisponible); // 50 - 2
            Assert.Equal(1, b2.CantidadDisponible); //  2 - 1
        }
    }
}
