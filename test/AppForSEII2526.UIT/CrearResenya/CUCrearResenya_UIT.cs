using AppForMovies.UIT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CrearResenya
{
    public class CUCrearResenya_UIT : UC_UIT
    {
        private SelectBocadillosParaResenyarPO selectBocadillosParaResenyar;
        private const string bocadilloNombre1 = "Politecnico";
        private const string bocadilloPVP1 = "10000";
        private const string tipoPan1 = "sandwich";
        private const string tamanyo1 = "Pequenyo";
        private const string bocadilloNombre2 = "Focata";
        private const string bocadilloPVP2 = "10";
        private const string tipoPan2 = "sandwich";
        private const string tamanyo2 = "Normal";



        public CUCrearResenya_UIT(ITestOutputHelper output) : base(output)
        {
            selectBocadillosParaResenyar = new SelectBocadillosParaResenyarPO(_driver, _output);
        }

        private void InitialStepsForResenyaBocadillo()
        {
            //we wait for the option of the menu to be visible
            selectBocadillosParaResenyar.WaitForBeingVisible(By.Id("hacerResenya"));
            //we click on the menu
            _driver.FindElement(By.Id("hacerResenya")).Click();
        }

        [Theory]
        [InlineData(bocadilloNombre1, tipoPan1, bocadilloPVP1, tamanyo1, "Pol", "0")]
        [InlineData(bocadilloNombre2, tipoPan2, bocadilloPVP2, tamanyo2, "", "10")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_FA1_2_filtros(string name, string tipopan, string pvp, string tamanyo,
            string filtroName, string filtroPVP)
        {
            //Arrange
            InitialStepsForResenyaBocadillo();
            var expectedBocadillos = new List<string[]> { new string[] { name, tipopan, pvp, tamanyo, "Añadir" }, };

            //Act
            selectBocadillosParaResenyar.SearchBocadillos(filtroName, filtroPVP);
            //Assert
            Assert.True(selectBocadillosParaResenyar.CheckListOfBocadillos(expectedBocadillos));

        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_FB_2_Bocadillos()
        {
            //Arrange
            InitialStepsForResenyaBocadillo();
            var expectedBocadillos = new List<string[]> { new string[] { bocadilloNombre2, tipoPan2, bocadilloPVP2, tamanyo2, "Añadir" }, 
                                                          new string[] { bocadilloNombre1, tipoPan1, bocadilloPVP1, tamanyo1, "Añadir" }, };
            //Act
            selectBocadillosParaResenyar.SearchBocadillos("", "");

            //Assert

            Assert.True(selectBocadillosParaResenyar.CheckListOfBocadillos(expectedBocadillos));

        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_FB_2_NoBocadillo()
        {
            //Arrange


            //Act

            InitialStepsForResenyaBocadillo();
            selectBocadillosParaResenyar.SearchBocadillos("", "1");

            //Assert
            Assert.True(selectBocadillosParaResenyar.CheckNotFound("No se ha encontrado ningún bocadillo"), $"No  se ha encontrado ningún bocadillo");
        }


    }
}
