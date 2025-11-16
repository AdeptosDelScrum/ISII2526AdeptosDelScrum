using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

using Microsoft.AspNetCore.Mvc;

using AppForSEII2526.UT;    // AppForSEII25264SqliteUT
using AppForSEII2526.API.DTOs;
using AppForSEII2526.Models;

namespace AppForSEII2526.UT.BonosBocadilloController_test
{
    public class GetCompraDetails_test : AppForSEII25264SqliteUT
    {
        private readonly long _compraIdSemilla;
        private readonly DateTime _fechaCompraUtc = DateTime.Parse("2025-11-12T12:00:00Z");

        public GetCompraDetails_test()
        {
            var tNormal = new TipoBocadillo { IdTipo = 1, NombreTipo = "normal" };
            _context.Add(tNormal);

            var bonoA = new BonoBocadillo
            {
                BonoId = 101,
                Nombre = "Bono Mixto 10",
                NBocadillos = 10,
                CantidadDisponible = 100,
                PVP = 3.00m,
                IdTipo = 1,
                TipoBocadillo = tNormal
            };
            var bonoB = new BonoBocadillo
            {
                BonoId = 102,
                Nombre = "Bono Integral 5",
                NBocadillos = 5,
                CantidadDisponible = 50,
                PVP = 4.00m,
                IdTipo = 1,
                TipoBocadillo = tNormal
            };
            _context.AddRange(bonoA, bonoB);

            var compra = new Compra();
            TrySetSilent(compra, "CompraId", 200L);
            TrySetSilent(compra, "NombreCompleto", "Pepe");
            TrySetSilent(compra, "Apellidos", "Perez");
            TrySetSilent(compra, "MetodoPago", "Tarjeta");
            TrySetSilent(compra, "FechaCompra", _fechaCompraUtc);
            _context.Add(compra);
            _context.SaveChanges();

            var compraId = GetAsLong(compra, "CompraId") ?? 0L;

            var l1 = new BonosComprados();
            TrySetSilent(l1, "CompraId", compraId);
            TrySetSilent(l1, "BonoId", 101);
            TrySetSilent(l1, "Cantidad", 2);
            TrySetSilent(l1, "PrecioUnitario", 5.00m);

            var l2 = new BonosComprados();
            TrySetSilent(l2, "CompraId", compraId);
            TrySetSilent(l2, "BonoId", 102);
            TrySetSilent(l2, "Cantidad", 3);

            _context.AddRange(l1, l2);
            _context.SaveChanges();

            _compraIdSemilla = compraId;
        }

        private dynamic CreateController()
        {
            var apiAsm = typeof(CrearCompraDTO).Assembly.GetName().Name; // AppForSEII2526.API
            var t =
                Type.GetType($"AppForSEII2526.API.Controllers.BonosBocadilloController, {apiAsm}")
                ?? Type.GetType($"AppForSEII2526.API.Controllers.BonoBocadilloController, {apiAsm}");
            if (t == null) throw new InvalidOperationException("No se encontró el controller de bonos.");
            var instance = Activator.CreateInstance(t, _context, null);
            if (instance == null) throw new InvalidOperationException("No se pudo instanciar el controller.");
            return instance;
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompraDetails_NotFound_test()
        {
            dynamic controller = CreateController();

            var result = await controller.GetCompraDetails(0L, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompraDetails_Found_test()
        {
            dynamic controller = CreateController();

            var result = await controller.GetCompraDetails(_compraIdSemilla, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<CompraDetailsDTO>(ok.Value);

            // Cabecera
            Assert.Equal(_compraIdSemilla, dto.CompraId);
            Assert.Equal("Pepe", dto.NombreCompleto);
            Assert.Equal("Perez", dto.Apellidos);
            Assert.Equal("Tarjeta", dto.MetodoPago);
            Assert.Equal(_fechaCompraUtc, dto.Fecha);

            // Buscar items SIN lambdas (evita CS1977 con dynamic)
            CompraBonoItemDTO iA = null;
            CompraBonoItemDTO iB = null;
            foreach (var it in dto.Items)
            {
                if (it.BonoId == 101) iA = it;
                else if (it.BonoId == 102) iB = it;
            }
            Assert.NotNull(iA);
            Assert.NotNull(iB);

            // Item A
            Assert.Equal("Bono Mixto 10", iA.Nombre);
            Assert.Equal("normal", iA.Tipo);
            Assert.Equal(5.00m, iA.Pvp);
            Assert.Equal(2, iA.Cantidad);

            // Item B
            Assert.Equal("Bono Integral 5", iB.Nombre);
            Assert.Equal("normal", iB.Tipo);
            Assert.Equal(4.00m, iB.Pvp);
            Assert.Equal(3, iB.Cantidad);

            var totalEsperado = (5.00m * 2) + (4.00m * 3);
            Assert.Equal(totalEsperado, dto.PrecioTotal);
        }

        // Helpers reflexión
        private static bool TrySetSilent(object entity, string prop, object? value)
        {
            var p = entity.GetType().GetProperty(prop);
            if (p == null || !p.CanWrite) return false;
            try
            {
                var target = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                var converted = value == null ? null : Convert.ChangeType(value, target);
                p.SetValue(entity, converted);
                return true;
            }
            catch { return false; }
        }

        private static long? GetAsLong(object entity, string prop)
        {
            var p = entity.GetType().GetProperty(prop);
            if (p == null || !p.CanRead) return null;
            try
            {
                var val = p.GetValue(entity);
                if (val == null) return null;
                return Convert.ToInt64(val);
            }
            catch { return null; }
        }
    }
}
