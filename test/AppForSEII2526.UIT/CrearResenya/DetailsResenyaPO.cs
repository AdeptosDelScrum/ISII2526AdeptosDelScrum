using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CrearResenya
{
    public class DetailsResenyaPO : PageObject
    {
        private By tableOfResenya0By = By.Id("Tablederesenya0");
        private By tableOfResenya1By = By.Id("Tablederesenya1");
        private By tableOfBocadillosBy = By.Id("TableOfBocadillos");
        protected DetailsResenyaPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public bool CheckResenyaData(List<string[]> tableLine0, List<string[]> tableLine1, List<string[]> bocadillos)
        {
            return CheckBodyTable(bocadillos, tableOfBocadillosBy) && CheckBodyTable(tableLine0, tableOfResenya0By) && CheckBodyTable(tableLine1, tableOfResenya1By);
        }
    }
}
