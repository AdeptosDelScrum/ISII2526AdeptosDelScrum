using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private By inputPuntuacion = By.Id("puntuacion_Focata");
        private By resenyarButton = By.Id("Submit");
        private By tableOfBocadillosBy = By.Id("TableOfBocadillos");
        private By okButtonBy = By.Id("Button_DialogOK");
        private By modificarButtonBy = By.Id("ModifyBocadillos");



        public CreateResenyaPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void resenyar(string name, string title, string description, string rate, string nameclient, string apellido1, string apellido2, string puntuacion)
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
            WaitForBeingClickable(inputPuntuacion);
            _driver.FindElement(inputPuntuacion).SendKeys(puntuacion);


            _driver.FindElement(resenyarButton).Click();
        }

        public bool CheckListOfBocadillos(List<string[]> expectedBocadillos)
        {

            return CheckBodyTable(expectedBocadillos, tableOfBocadillosBy);
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

    }
}
