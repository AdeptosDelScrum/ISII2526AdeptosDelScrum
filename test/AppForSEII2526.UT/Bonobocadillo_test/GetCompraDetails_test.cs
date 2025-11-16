using AppForSEII2526.UT;                    // AppForSEII25264SqliteUT
using AppForSEII2526.API.Controllers;       // BonosBocadilloController
using AppForSEII2526.API.DTOs;              // CompraDetailsDTO
using AppForSEII2526.Models;                // BonoBocadillo, TipoBocadillo, BonosComprados, Compra
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.BonosBocadilloController_test
{
    public class GetCompraDetails_test : AppForSEII25264SqliteUT
    {
        private readonly long _compraIdSemilla;
        private readonly DateTime _fechaCompraUtc = DateTime.Parse("2025-11-12T12:00:00Z");

        public GetCompraDetails_test()
        {
            // Tipos
            var tNormal = new TipoBocadillo { IdTipo = 1, NombreTipo = "normal" };
            _context.Add(tNormal);

            // Bonos
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

            // Compra (usamos reflexion para adaptarnos a nombres reales de props)
            var compra = new Compra();
            TrySetSilent(compra, "CompraId", 200L); // si la PK es identity, se ignorara
            TrySetSilent(compra, "NombreCompleto", "Pepe");
            TrySetSilent(compra, "Apellidos", "Perez");
            // Metodo de pago como string (si tu modelo usa entidad, el controller deberia mapear a string)
            TrySetSilent(compra, "MetodoPago", "Tarjeta");
            TrySetSilent(compra, "MetodoPagoNombre", "Tarjeta"); // por si el campo se llama asi
            TrySetSilent(compra, "FechaCompra", _fechaCompraUtc);
            TrySetSilent(compra, "Fecha", _fechaCompraUtc);      // por si usa "Fecha"
            _context.Add(compra);
            _context.SaveChanges();

            var compraId = GetAsLong(compra, "CompraId") ?? 0L;

            // Lineas: usamos tabla intermedia BonosComprados
            var l1 = new BonosComprados();
            TrySetSilent(l1, "CompraId", compraId);
            TrySetSilent(l1, "BonoId", 101);
            TrySetSilent(l1, "Cantidad", 2);
            TrySetSilent(l1, "PrecioUnitario", 5.00m); // fuerza uso de precio de linea

            var l2 = new BonosComprados();
            TrySetSilent(l2, "CompraId", compraId);
            TrySetSilent(l2, "BonoId", 102);
            TrySetSilent(l2, "Cantidad", 3);
            // sin PrecioUnitario para que use PVP del bono (4.00)

            _context.AddRange(l1, l2);
            _context.SaveChanges();

            _compraIdSemilla = compraId;
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompraDetails_NotFound_test()
        {
            var mock = new Mock<ILogger<BonosBocadilloController>>();
            var controller = new BonosBocadilloController(_context, mock.Object);

            var result = await controller.GetCompraDetails(0, CancellationToken.None);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetCompraDetails_Found_test()
        {
            var mock = new Mock<ILogger<BonosBocadilloController>>();
            var controller = new BonosBocadilloController(_context, mock.Object);

            var result = await controller.GetCompraDetails(_compraIdSemilla, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<CompraDetailsDTO>(ok.Value);

            // Cabecera
            Assert.Equal(_compraIdSemilla, dto.CompraId);
            Assert.Equal("Pepe", dto.NombreCompleto);
            Assert.Equal("Perez", dto.Apellidos);
            Assert.Equal("Tarjeta", dto.MetodoPago);

            // Fecha (segun como la mapees, comparamos fecha o exacto)
            Assert.Equal(_fechaCompraUtc, dto.Fecha);

            // Items
            Assert.Equal(2, dto.Items.Count);

            var iA = dto.Items.Single(i => i.BonoId == 101);
            Assert.Equal("Bono Mixto 10", iA.Nombre);
            Assert.Equal("normal", iA.Tipo);
            Assert.Equal(5.00m, iA.Pvp);  // toma PrecioUnitario de la linea
            Assert.Equal(2, iA.Cantidad);

            var iB = dto.Items.Single(i => i.BonoId == 102);
            Assert.Equal("Bono Integral 5", iB.Nombre);
            Assert.Equal("normal", iB.Tipo);
            Assert.Equal(4.00m, iB.Pvp);  // sin PrecioUnitario -> cae a PVP del bono
            Assert.Equal(3, iB.Cantidad);

            var totalEsperado = (5.00m * 2) + (4.00m * 3);
            Assert.Equal(totalEsperado, dto.PrecioTotal);
        }

        // ===== helpers por reflexion (sin tildes en codigo) =====
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
