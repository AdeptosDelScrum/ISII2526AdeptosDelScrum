using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace AppForSEII2526.UIT.CrearResenya
{
    public class SelectBocadillosParaResenyarPO : PageObject
    {
        private By inputNombre = By.Id("inputNombre");
        private By inputpvp = By.Id("inputPVP");
        private By buscar = By.Id("searchBocadillo");
        private By tableOfBocadillosBy = By.Id("TableOfBocadillos");
        private By errorShownBy = By.Id("ErrorsShown");
        private By messageShownBy = By.Id("notFound");
        private By continueButtonBy = By.Id("ResenyarBocadilloButton");
        public SelectBocadillosParaResenyarPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
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

        public bool CheckNotFound(string NotFoundMessage)
        {
            WaitForBeingVisible(By.Id("notFound"));
            IWebElement actualMessageShown = _driver.FindElement(messageShownBy);
            _output.WriteLine($"actual Message shown:{actualMessageShown.Text}");
            return actualMessageShown.Text.Contains(NotFoundMessage);
        }

        public void addBocadillo(string name)
        {
            By addButtonBy = By.Id("bocadilloParaResenyar_" + name);
            WaitForBeingClickable(addButtonBy);
            _driver.FindElement(addButtonBy).Click();
        }

        public void removeBocadillo(string name)
        {
            By removeButtonBy = By.Id("removeBocadillo_" + name);
            WaitForBeingClickable(removeButtonBy);
            _driver.FindElement(removeButtonBy).Click();
        }

        public void continueToResenyar()
        {
            WaitForBeingClickable(continueButtonBy);
            _driver.FindElement(continueButtonBy).Click();
        }

    }
}
