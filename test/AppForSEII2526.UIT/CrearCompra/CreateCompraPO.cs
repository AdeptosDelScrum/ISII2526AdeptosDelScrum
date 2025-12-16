using OpenQA.Selenium;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.CrearCompra
{
    public class CreateCompraPO : PageObject
    {
      
        private By nombreBy = By.Id("Nombre");
        private By apellido1By = By.Id("Apellido1");
        private By apellido2By = By.Id("Apellido2");
        private By metodoPagoBy = By.Id("MetodoPago");

   
        private By submitBy = By.Id("Submit");
        private By modifyBocadillosBy = By.Id("ModifyBocadillos");


        private By errorShownBy = By.Id("ErrorsShown");


        private By tableOfItemsBy = By.Id("TableOfRentalItems");

        public CreateCompraPO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }


        public void FillCustomerData(string nombre, string apellido1, string? apellido2)
        {
            WaitForBeingVisible(nombreBy);
            _driver.FindElement(nombreBy).Clear();
            _driver.FindElement(nombreBy).SendKeys(nombre);

            _driver.FindElement(apellido1By).Clear();
            _driver.FindElement(apellido1By).SendKeys(apellido1);

            if (apellido2 != null)
            {
                _driver.FindElement(apellido2By).Clear();
                _driver.FindElement(apellido2By).SendKeys(apellido2);
            }
        }

        public void SelectMetodoPago(int metodoPagoId)
        {
            WaitForBeingClickable(metodoPagoBy);
            SelectElement select = new SelectElement(_driver.FindElement(metodoPagoBy));
            select.SelectByValue(metodoPagoId.ToString());
        }

 
        public bool CheckBocadillosInCart(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, tableOfItemsBy);
        }

        public void ChangeCantidad(string bocadilloNombre, int cantidad)
        {
            By rowBy = By.Id($"BocadillosData_{bocadilloNombre}");
            WaitForBeingVisible(rowBy);

            IWebElement row = _driver.FindElement(rowBy);
            IWebElement inputCantidad = row.FindElement(By.TagName("input"));

            inputCantidad.Clear();
            inputCantidad.SendKeys(cantidad.ToString());
        }


        public void SubmitCompra()
        {
            WaitForBeingClickable(submitBy);
            _driver.FindElement(submitBy).Click();
        }

        public void ModifyBocadillos()
        {
            WaitForBeingClickable(modifyBocadillosBy);
            _driver.FindElement(modifyBocadillosBy).Click();
        }

        private By dialogSaveButton = By.Id("Button_DialogOK");
        private By dialogDontSaveButton = By.Id("Button_DialogCancel");

        public void ConfirmCompra()
        {
            WaitForBeingClickable(dialogSaveButton);
            _driver.FindElement(dialogDontSaveButton).Click();
        }
        public bool CheckError(string expectedError)
        {
            WaitForBeingVisible(errorShownBy);
            return _driver.FindElement(errorShownBy).Text.Contains(expectedError);
        }
    }
}
