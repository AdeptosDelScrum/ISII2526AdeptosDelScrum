using AppForMovies.UIT.Shared;
using AppForSEII2526.UIT.Compras;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.UIT.CrearCompra
{
    public class CUCrearCompra_UIT : UC_UIT
    {
        private SelectBocadillosParaCompraPO selectBocadillosParaCompra;
        private CreateCompraPO createCompra;
        private DetailsCompraPO detailsCompra;

        private const string bocadilloNombre1 = "Completo";
        private const string bocadilloPrecio1 = "3";
        private const string tipoPan1 = "Normal";

        private const string bocadilloNombre2 = "Americano";
        private const string bocadilloPrecio2 = "4";
        private const string tipoPan2 = "Integral";

        private const string nombreCliente = "Paco";
        private const string apellido1 = "Olivares";
        private const string apellido2 = "Alonso";

        public CUCrearCompra_UIT(ITestOutputHelper output) : base(output)
        {
            selectBocadillosParaCompra = new SelectBocadillosParaCompraPO(_driver, _output);
            createCompra = new CreateCompraPO(_driver, _output);
            detailsCompra = new DetailsCompraPO(_driver, _output);
        }

        private void InitialStepsForCompra()
        {
            
            selectBocadillosParaCompra.WaitForBeingVisible(By.LinkText("Compra"));

            
            _driver.FindElement(By.LinkText("Compra")).Click();
        }
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Compra_FB_FA0_Filtros()
        {
            InitialStepsForCompra();

            var expectedBocadillos = new List<string[]> {
                new string[] { "Politecnico", "Normal", "Baguette","2", "Añadir" }
            };

            selectBocadillosParaCompra.SearchBocadillos("", "Normal");

            Assert.True(selectBocadillosParaCompra.CheckListOfBocadillos(expectedBocadillos));
        }

        
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Compra_FB_ListadoBocadillos()
        {
            InitialStepsForCompra();

            var expectedBocadillos = new List<string[]> {
                new string[] { "Completo", "Pequenyo", "Barra","3", "Añadir" },
                new string[] { "Politecnico", "Normal", "Baguette","2", "Añadir" }
            };

            selectBocadillosParaCompra.SearchBocadillos("", "");

            Assert.True(selectBocadillosParaCompra.CheckListOfBocadillos(expectedBocadillos));
        }


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Compra_Examen()
        {
            InitialStepsForCompra();

            var expectedBocadillos = new List<string[]> {
                new string[] { "x", "1", "Completo","-","3€" },
                new string[] { "x", "1", "Politecnico", "-", "2€" }
            };
            selectBocadillosParaCompra.SearchBocadillos("", "");
            selectBocadillosParaCompra.AddBocadillo("Completo");

            selectBocadillosParaCompra.SearchBocadillos("Baguette", "");
            selectBocadillosParaCompra.AddBocadillo("Politecnico");

            string b1 = "x"+"1"+ "Completo"+"-"+"3€";
            string b2 = "x"+ "1"+ "Politecnico"+ "-"+ "2€";

            Assert.True(selectBocadillosParaCompra.CheckCarrito(b1));
            Assert.True(selectBocadillosParaCompra.CheckCarrito(b2));

        }
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Compra_FB_FlujoCompleto()
        {
            InitialStepsForCompra();

            selectBocadillosParaCompra.SearchBocadillos("", "");
            selectBocadillosParaCompra.AddBocadillo(bocadilloNombre1);
            selectBocadillosParaCompra.ContinueToCompra();

            createCompra.FillCustomerData(nombreCliente, apellido1, apellido2);
            createCompra.SelectMetodoPago(1); 
            createCompra.ChangeCantidad(bocadilloNombre1, 2);
            createCompra.SubmitCompra();
            createCompra.ConfirmCompra();
            var expectedBocadillos = new List<string[]>
            {
                new[] { bocadilloNombre1, tipoPan1, "2", bocadilloPrecio1 }
            };

            Assert.True(detailsCompra.CheckCompraData(
                nombreCliente,
                apellido1,
                apellido2,
                "Tarjeta",
                expectedBocadillos
            ));
        }
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Compra_FA1_SinBocadillos()
        {
            InitialStepsForCompra();

            Assert.True(selectBocadillosParaCompra.CheckHideCart());
        }
        [Theory]
        [InlineData("", "Olivares", "Debe ingrsar su nombre")]
        [InlineData("Paco", "", "Debe ingrsar su primer apellido")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Compra_FA3_ErroresFormulario(string nombre, string apellido, string error)
        {
            InitialStepsForCompra();

            selectBocadillosParaCompra.SearchBocadillos("", "");
            selectBocadillosParaCompra.AddBocadillo(bocadilloNombre1);
            selectBocadillosParaCompra.ContinueToCompra();

            createCompra.FillCustomerData(nombre, apellido, "");
            createCompra.SelectMetodoPago(-1);
            createCompra.SubmitCompra();

            Assert.True(createCompra.CheckError(error));
        }
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_Compra_FA2_ModificarBocadillos()
        {
            InitialStepsForCompra();

            selectBocadillosParaCompra.SearchBocadillos("", "");
            selectBocadillosParaCompra.AddBocadillo(bocadilloNombre1);
            selectBocadillosParaCompra.AddBocadillo(bocadilloNombre2);

            selectBocadillosParaCompra.RemoveBocadillo(bocadilloNombre1);

            var expected = new List<string[]>
            {
                new[] { bocadilloNombre2, tipoPan2, bocadilloPrecio2, "Añadir" }
            };

            Assert.True(selectBocadillosParaCompra.CheckListOfBocadillos(expected));
        }
    }
}