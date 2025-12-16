using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Compras
{
    public class SelectBocadillosParaCompraPO : PageObject
    {
        
        private By inputTipoPan = By.Id("inputTipo");
        private By selectTamanyo = By.Id("selectTamanyo");
        private By buscar = By.Id("searchBocadillos");

        
        private By tableOfBocadillosBy = By.Id("TableOfBocadillos");
        private By errorShownBy = By.Id("ErrorsShown");
        private By notFoundBy = By.Id("notFound");

        
        private By continueButtonBy = By.Id("purchaseMovieButton");

        public SelectBocadillosParaCompraPO(
            IWebDriver driver,
            ITestOutputHelper output
        ) : base(driver, output)
        {
        }

        public void SearchBocadillos(string tipoPan, string tamanyo)
        {
            if (!string.IsNullOrEmpty(tipoPan))
            {
                var inputTipoPan = _driver.FindElement(By.Id("inputTipo"));
                inputTipoPan.Clear();
                inputTipoPan.SendKeys(tipoPan);
            }

            if (!string.IsNullOrEmpty(tamanyo))
            {
                var selectTamanyo = new SelectElement(
                    _driver.FindElement(By.Id("selectTamanyo"))
                );
                selectTamanyo.SelectByText(tamanyo);
            }

            _driver.FindElement(By.Id("searchBocadillos")).Click();
        }

        public void AddBocadillo(string nombre)
        {
            By addButtonBy = By.Id("bocadilloParaComprar_" + nombre);
            WaitForBeingClickable(addButtonBy);
            _driver.FindElement(addButtonBy).Click();
        }

        public void RemoveBocadillo(string nombre)
        {
            By removeButtonBy = By.Id("removeBocadillo_" + nombre);
            WaitForBeingClickable(removeButtonBy);
            _driver.FindElement(removeButtonBy).Click();
            ImplicitWait(2);
        }

        public void ContinueToCompra()
        {
            WaitForBeingClickable(continueButtonBy);
            _driver.FindElement(continueButtonBy).Click();
        }

        public bool CheckListOfBocadillos(List<string[]> expectedBocadillos)
        {
            return CheckBodyTable(expectedBocadillos, tableOfBocadillosBy);
        }

        public bool CheckError(string expectedError)
        {
            WaitForBeingVisible(errorShownBy);
            var error = _driver.FindElement(errorShownBy);
            _output.WriteLine($"Error mostrado: {error.Text}");
            return error.Text.Contains(expectedError);
        }

        public bool CheckNotFound(string expectedMessage)
        {
            WaitForBeingVisible(notFoundBy);
            var message = _driver.FindElement(notFoundBy);
            _output.WriteLine($"Mensaje mostrado: {message.Text}");
            return message.Text.Contains(expectedMessage);
        }

        public bool CheckHideCart()
        {
            IWebElement boton = _driver.FindElement(continueButtonBy);
            return !boton.Displayed;
        }
    }
}
