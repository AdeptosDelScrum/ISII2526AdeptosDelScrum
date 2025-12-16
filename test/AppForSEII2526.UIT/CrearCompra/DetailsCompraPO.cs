using OpenQA.Selenium;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.CrearCompra
{
    public class DetailsCompraPO : PageObject
    {
      
      
        private By tableOfBocadillosBy = By.Id("BocadillosComprados");

       
        private By nombreApellidosBy = By.Id("NombreApellidos");
        private By metodoPagoBy = By.Id("MetodoPago");
        private By fechaCompraBy = By.Id("RentalDate");
        private By totalPriceBy = By.Id("TotalPrice");

        public DetailsCompraPO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

      
        public bool CheckCompraHeader(
            string nombreApellidos,
            string metodoPago,
            string fecha)
        {
            WaitForBeingVisible(nombreApellidosBy);

            return _driver.FindElement(nombreApellidosBy).Text.Contains(nombreApellidos)
                && _driver.FindElement(metodoPagoBy).Text.Contains(metodoPago)
                && _driver.FindElement(fechaCompraBy).Text.Contains(fecha);
        }

       
        public bool CheckBocadillosComprados(List<string[]> expectedBocadillos)
        {
            return CheckBodyTable(expectedBocadillos, tableOfBocadillosBy);
        }

        public bool CheckTotalPrice(string expectedPrice)
        {
            return _driver.FindElement(totalPriceBy).Text.Contains(expectedPrice);
        }


        public bool CheckCompraData(
            string nombre,
            string apellido1,
            string apellido2,
            string metodoPago,
            List<string[]> bocadillos)
        {
            // Nombre completo
            var nombreCompleto = $"{nombre} {apellido1} {apellido2}";
            WaitForBeingVisible(nombreApellidosBy);
            var nombreActual = _driver.FindElement(nombreApellidosBy).Text;

            _output.WriteLine($"Nombre mostrado: {nombreActual}");
            if (!nombreActual.Contains(nombreCompleto))
                return false;

            // Método de pago
            var metodoActual = _driver.FindElement(metodoPagoBy).Text;
            _output.WriteLine($"Método de pago mostrado: {metodoActual}");
            if (!metodoActual.Contains(metodoPago))
                return false;

            // Tabla de bocadillos
            if (!CheckBodyTable(bocadillos, tableOfBocadillosBy))
                return false;

            return true;
        }
    }
}
