using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CrearResenya
{
    internal class SelectBocadillosParaResenyarPO : PageObject
    {
        private By inputNombre = By.Id("inputNombre");
        private By inputpvp = By.Id("inputPVP");
        private By buscar = By.Id("searchBocadillo");
        private By tableOfBocadillosBy = By.Id("TableOfBocadillos");
        By errorShownBy = By.Id("ErrorsShown");
        protected SelectBocadillosParaResenyarPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }
        public void SearchBocadillos(string nombre, string? pvp)
        {
            //wait for the webelement to be clickable
            WaitForBeingClickable(inputNombre);
            _driver.FindElement(inputNombre).SendKeys(nombre);
            WaitForBeingClickable(inputpvp);
            _driver.FindElement(inputpvp).SendKeys(pvp);

            _driver.FindElement(buscar).Click();


        }

        public bool CheckListOfBocadillos(List<string[]> expectedBocadillos)
        {

            return CheckBodyTable(expectedBocadillos, tableOfBocadillosBy);
        }

        public bool CheckMessageError(string errorMessage)
        {
            IWebElement actualErrorShown = _driver.FindElement(errorShownBy);
            _output.WriteLine($"actual Message shown:{actualErrorShown.Text}");
            return actualErrorShown.Text.Contains(errorMessage);
        }

    }
}
