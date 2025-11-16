using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Moq;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using AppForSEII2526.UT;                     // AppForSEII25264SqliteUT
using AppForSEII2526.API.Controllers;        // BonosBocadilloController
using AppForSEII2526.API.DTOs;               // BonoBocadilloDTO
using AppForSEII2526.Models;                 // BonoBocadillo, TipoBocadillo

namespace AppForSEII2526.UT.BonosBocadilloController_test
{
    public class GetBonosDisponiblesSelect_test : AppForSEII25264SqliteUT
    {
        // ids usados en las expectativas
        private const int ID_B1 = 1;  // normal, stock>0, nombre "Bono Mixto 10", PVP 3.50
        private const int ID_B2 = 2;  // vegano, stock=0  -> NO debe salir
        private const int ID_B3 = 3;  // vegano, stock>0, nombre "Pack Vegano 8", PVP 3.80
        private const int ID_B4 = 4;  // normal, stock>0, nombre "Mixto Especial", PVP 5.00

        public GetBonosDisponiblesSelect_test()
        {
            // ----- Semilla mínima de datos -----
            var tNormal = new TipoBocadillo { IdTipo = 10, NombreTipo = "normal" };
            var tVegano = new TipoBocadillo { IdTipo = 20, NombreTipo = "vegano" };
            _context.AddRange(tNormal, tVegano);

            var b1 = new BonoBocadillo
            {
                BonoId = ID_B1,
                Nombre = "Bono Mixto 10",
                NBocadillos = 10,
                CantidadDisponible = 5,
                PVP = 3.50m,
                IdTipo = tNormal.IdTipo,
                TipoBocadillo = tNormal
            };
            var b2 = new BonoBocadillo
            {
                BonoId = ID_B2,
                Nombre = "Bono Vegano 5",
                NBocadillos = 5,
                CantidadDisponible = 0,   // sin stock -> no debe aparecer
                PVP = 4.00m,
                IdTipo = tVegano.IdTipo,
                TipoBocadillo = tVegano
            };
            var b3 = new BonoBocadillo
            {
                BonoId = ID_B3,
                Nombre = "Pack Vegano 8",
                NBocadillos = 8,
                CantidadDisponible = 7,
                PVP = 3.80m,
                IdTipo = tVegano.IdTipo,
                TipoBocadillo = tVegano
            };
            var b4 = new BonoBocadillo
            {
                BonoId = ID_B4,
                Nombre = "Mixto Especial",
                NBocadillos = 12,
                CantidadDisponible = 2,
                PVP = 5.00m,
                IdTipo = tNormal.IdTipo,
                TipoBocadillo = tNormal
            };

            _context.AddRange(b1, b2, b3, b4);
            _context.SaveChanges();
        }

        // -------- Casos OK con filtros (tipo, search) y resultado esperado (ids en orden) --------
        public static IEnumerable<object[]> TestCasesFor_GetBonosDisponiblesSelect_Ok()
        {
            // Orden esperado por Nombre ASC:
            // "Bono Mixto 10" (ID_B1), "Mixto Especial" (ID_B4), "Pack Vegano 8" (ID_B3)
            yield return new object[] { null,       null,        new[] { ID_B1, ID_B4, ID_B3 } }; // todos con stock
            yield return new object[] { "vegano",   null,        new[] { ID_B3 } };               // solo veganos con stock
            yield return new object[] { "NORMAL",   null,        new[] { ID_B1, ID_B4 } };        // case-insensitive
            yield return new object[] { null,       "Mixto",     new[] { ID_B1, ID_B4 } };        // search por nombre
            yield return new object[] { "vegano",   "Mixto",     Array.Empty<int>() };             // combinación sin resultados
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetBonosDisponiblesSelect_Ok))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetBonosDisponiblesSelect_Ok_test(string? tipo, string? search, int[] expectedIdsInOrder)
        {
            // Arrange
            var logger = new Mock<ILogger<BonosBocadilloController>>().Object;
            var controller = new BonosBocadilloController(_context, logger);

            // Act
            var result = await controller.GetBonosDisponiblesSelect(tipo, search);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<BonoBocadilloDTO>>(ok.Value);

            // Comprobar orden por Nombre ASC y que solo hay stock>0
            var ids = list.Select(x => (int)x.BonoId).ToArray();
            Assert.Equal(expectedIdsInOrder, ids);

            // Ninguno con stock 0
            // (si tu DTO no expone CantidadDisponible, omite esta comprobacion)
            foreach (var dto in list)
            {
                // si existiera prop CantidadDisponible en el DTO
                var stockProp = dto.GetType().GetProperty("CantidadDisponible");
                if (stockProp != null)
                {
                    var stock = (int)stockProp.GetValue(dto)!;
                    Assert.True(stock > 0);
                }
            }
        }

        // -------- Validacion de mapeo de campos (un item concreto) --------
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetBonosDisponiblesSelect_MapeoCampos_test()
        {
            var logger = new Mock<ILogger<BonosBocadilloController>>().Object;
            var controller = new BonosBocadilloController(_context, logger);

            var result = await controller.GetBonosDisponiblesSelect(null, "Bono Mixto 10");
            var ok = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<BonoBocadilloDTO>>(ok.Value);
            var dto = Assert.Single(list);

            Assert.Equal(ID_B1, dto.BonoId);
            Assert.Equal("Bono Mixto 10", dto.Nombre);
            Assert.Equal(10, dto.NBocadillos);
            Assert.Equal(3.50m, dto.Pvp);

            // nombre del tipo puede venir en dto.NombreTipo o en dto.Tipo.NombreTipo (segun tu DTO)
            var nombreTipo = GetProp<string>(dto, "NombreTipo") 
                             ?? GetProp<string>(dto, "Tipo.NombreTipo");
            Assert.Equal("normal", nombreTipo);
        }

        // ===== helper para leer propiedades (incluye rutas con punto p.e. "Tipo.NombreTipo") =====
        private static T? GetProp<T>(object obj, string path)
        {
            object? current = obj;
            foreach (var seg in path.Split('.'))
            {
                if (current == null) return default;
                var pi = current.GetType().GetProperty(seg);
                if (pi == null) return default;
                current = pi.GetValue(current);
            }
            if (current == null) return default;
            return (T)Convert.ChangeType(current, typeof(T));
        }
    }
}
