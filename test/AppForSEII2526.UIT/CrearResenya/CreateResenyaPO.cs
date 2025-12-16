using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.UIT.CrearResenya
{
    public class CreateResenyaPO : PageObject
    {
        private By inputNombre = By.Id("Name");
        private By inputTitle = By.Id("Title");
        private By inputDescription = By.Id("Description");
        private By inputRate = By.Id("Rate");
        private By inputNombreClient = By.Id("NameClient");
        private By inputApellido1 = By.Id("Apellido1");
        private By inputApellido2 = By.Id("Apellido2");
        private By resenyarButton = By.Id("Submit");
        private By ModifyBocadillosBy = By.Id("ModifyBocadillos");
        private By okButtonBy = By.Id("Button_DialogOK");
        private By modificarButtonBy = By.Id("ModifyBocadillos");
        private By errorShownBy = By.Id("ErrorsShown");



        public CreateResenyaPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void resenyar(string name, string title, string description, string rate, string nameclient, string apellido1, string apellido2, List<List<string>> parBocaPunt)
        {
            WaitForBeingClickable(inputNombre);
            _driver.FindElement(inputNombre).SendKeys(name);
            WaitForBeingClickable(inputTitle);
            _driver.FindElement(inputTitle).SendKeys(title);
            WaitForBeingClickable(inputDescription);
            _driver.FindElement(inputDescription).SendKeys(description);
            WaitForBeingClickable(inputRate);
            _driver.FindElement(inputRate).SendKeys(rate);
            WaitForBeingClickable(inputNombreClient);
            _driver.FindElement(inputNombreClient).SendKeys(nameclient);
            WaitForBeingClickable(inputApellido1);
            _driver.FindElement(inputApellido1).SendKeys(apellido1);
            WaitForBeingClickable(inputApellido2);
            _driver.FindElement(inputApellido2).SendKeys(apellido2);

            for (int i = 0; i < parBocaPunt.Count; i++)
            {
                By inputPuntuacion = By.Id("puntuacion_" + parBocaPunt[i][0]);
                WaitForBeingClickable(inputPuntuacion);
                _driver.FindElement(inputPuntuacion).SendKeys(parBocaPunt[i][1]);
            }
            
            createResenya();
        }

        public void soloRellenar(string name, string title, string description, string rate, string nameclient, string apellido1, string apellido2, List<List<string>> parBocaPunt)
        {
            WaitForBeingClickable(inputNombre);
            _driver.FindElement(inputNombre).SendKeys(name);
            WaitForBeingClickable(inputTitle);
            _driver.FindElement(inputTitle).SendKeys(title);
            WaitForBeingClickable(inputDescription);
            _driver.FindElement(inputDescription).SendKeys(description);
            WaitForBeingClickable(inputRate);
            _driver.FindElement(inputRate).SendKeys(rate);
            WaitForBeingClickable(inputNombreClient);
            _driver.FindElement(inputNombreClient).SendKeys(nameclient);
            WaitForBeingClickable(inputApellido1);
            _driver.FindElement(inputApellido1).SendKeys(apellido1);
            WaitForBeingClickable(inputApellido2);
            _driver.FindElement(inputApellido2).SendKeys(apellido2);

            for (int i = 0; i < parBocaPunt.Count; i++)
            {
                By inputPuntuacion = By.Id("puntuacion_" + parBocaPunt[i][0]);
                WaitForBeingClickable(inputPuntuacion);
                _driver.FindElement(inputPuntuacion).SendKeys(parBocaPunt[i][1]);
            }
        }

        public bool checkData(string name, string title, string description, string rate, string nameclient, string apellido1, string apellido2, List<List<string>> parBocaPunt)
        {
            WaitForBeingVisible(inputNombre);
            IWebElement actualinputNombre = _driver.FindElement(inputNombre);
            if (!actualinputNombre.GetAttribute("value").Contains(name))
            {
                return false;
            }
            WaitForBeingVisible(inputTitle);
            IWebElement actualinputTitle = _driver.FindElement(inputTitle);
            if (!actualinputTitle.GetAttribute("value").Contains(title))
            {
                return false;
            }
            WaitForBeingVisible(inputDescription);
            IWebElement actualinputDescription = _driver.FindElement(inputDescription);
            if (!actualinputDescription.GetAttribute("value").Contains(description))
            {
                return false;
            }
            WaitForBeingVisible(inputRate);
            IWebElement actualinputRate = _driver.FindElement(inputRate);
            if (!actualinputRate.GetAttribute("value").Contains(rate))
            {
                return false;
            }
            WaitForBeingVisible(inputNombreClient);
            IWebElement actualinputNombreClient = _driver.FindElement(inputNombreClient);
            if (!actualinputNombreClient.GetAttribute("value").Contains(nameclient))
            {
                return false;
            }
            WaitForBeingVisible(inputApellido1);
            IWebElement actualinputApellido1 = _driver.FindElement(inputApellido1);
            if (!actualinputApellido1.GetAttribute("value").Contains(apellido1))
            {
                return false;
            }
            WaitForBeingVisible(inputApellido2);
            IWebElement actualinputApellido2 = _driver.FindElement(inputApellido2);
            if (!actualinputApellido2.GetAttribute("value").Contains(apellido2))
            {
                return false;
            }

            for (int i = 0; i < parBocaPunt.Count; i++)
            {
                By inputPuntuacion = By.Id("puntuacion_" + parBocaPunt[i][0]);
                WaitForBeingVisible(inputPuntuacion);
                IWebElement actualinputPuntuacion = _driver.FindElement(inputPuntuacion);
                var valor = actualinputPuntuacion.GetAttribute("value");
                if (!valor.Contains(parBocaPunt[i][1]))
                {
                    return false;
                }
            }
            return true;
        }

        public void createResenya()
        {
            _driver.FindElement(resenyarButton).Click();
            WaitForBeingClickable(okButtonBy);
            _driver.FindElement(okButtonBy).Click();
        }

        public void modificarBocadillo()
        {
            _driver.FindElement(modificarButtonBy).Click();
        }

        public bool checkErrors(string errorMessage)
        {
            IWebElement actualErrorShown = _driver.FindElement(errorShownBy);
            _output.WriteLine($"actual Message shown:{actualErrorShown.Text}");
            return actualErrorShown.Text.Contains(errorMessage);
        }

        public void modificarBocadillos()
        {
            _driver.FindElement(ModifyBocadillosBy).Click();
        }

    }
}
