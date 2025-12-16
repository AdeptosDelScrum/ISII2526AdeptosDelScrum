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
        private CreateResenyaPO createResenya;
        private DetailsResenyaPO detailsResenya;
        private const string bocadilloNombre1 = "Politecnico";
        private const string bocadilloPVP1 = "10000";
        private const string tipoPan1 = "sandwich";
        private const string tamanyo1 = "Pequenyo";
        private const string bocadilloNombre2 = "Focata";
        private const string bocadilloPVP2 = "10";
        private const string tipoPan2 = "sandwich";
        private const string tamanyo2 = "Normal";
        private const string nombreU = "Pedro";
        private const string title = "Mi primera reseña";
        private const string descripcion = "Me gustaría que tuviera más lechuga";
        private const string rate = "5";
        private const string puntuacion = "10";
        private const string nombreC = "Pedro";
        private const string apellido1C = "Pascal";
        private const string apellido2C = "No se";



        public CUCrearResenya_UIT(ITestOutputHelper output) : base(output)
        {
            selectBocadillosParaResenyar = new SelectBocadillosParaResenyarPO(_driver, _output);
            createResenya = new CreateResenyaPO(_driver, _output);
            detailsResenya = new DetailsResenyaPO(_driver, _output);
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
        public void UC2_FB_FA0_2_filtros(string name, string tipopan, string pvp, string tamanyo,
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
            selectBocadillosParaResenyar.SearchBocadillos("", "0");

            //Assert

            Assert.True(selectBocadillosParaResenyar.CheckListOfBocadillos(expectedBocadillos));

        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_FB_2_NoBocadillo()
        {
            //Arrange

            InitialStepsForResenyaBocadillo();
            //Act


            selectBocadillosParaResenyar.SearchBocadillos("", "1");

            //Assert
            Assert.True(selectBocadillosParaResenyar.CheckNotFound("No se ha encontrado ningún bocadillo"), $"No  se ha encontrado ningún bocadillo");
        }


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_FB()
        {
            //Arrange
            InitialStepsForResenyaBocadillo();
            selectBocadillosParaResenyar.SearchBocadillos("", "0");
            selectBocadillosParaResenyar.addBocadillo(bocadilloNombre2);
            selectBocadillosParaResenyar.continueToResenyar();

            var lineas = new List<List<string>> {
                new List<string> { bocadilloNombre2, "10" },
            };

            var expectedResenyaLine0 = new List<string[]> { new string[] { nombreU, title, DateTime.Today.ToString() + " +01:00" }, };
            var expectedResenyaLine1= new List<string[]> { new string[] { descripcion }, };
            var expectedResenyaBocadillos = new List<string[]> { new string[] { bocadilloNombre2, bocadilloPVP2, tamanyo2, lineas[0][1] }, };

            //Act

            createResenya.resenyar(nombreU,title, descripcion,rate,nombreC,apellido1C,apellido2C, lineas);

            //Assert

            Assert.True(detailsResenya.CheckResenyaData(expectedResenyaLine0, expectedResenyaLine1, expectedResenyaBocadillos));

        }

        [Theory]
        [InlineData("", descripcion, rate, puntuacion, "La reseña tiene que tener un título.")]
        [InlineData(title, "", rate, puntuacion, "La descripción debe empezar por me gustaría que.")]
        [InlineData(title, descripcion, "7", puntuacion, "La valoración general tiene que ser entre 1 y 5 estrellas.")]
        [InlineData(title, descripcion, rate, "11", "La puntuación debe ser un valor numérico entre 0 y 10")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_FB_FA3(string titulo, string descipcion, string rat, string punt, string error)
        {
            //Arrange
            InitialStepsForResenyaBocadillo();
            selectBocadillosParaResenyar.SearchBocadillos("", "0");
            selectBocadillosParaResenyar.addBocadillo(bocadilloNombre2);
            selectBocadillosParaResenyar.continueToResenyar();

            var lineas = new List<List<string>> {
                new List<string> { bocadilloNombre2, punt },
            };

            var expectedError = error;

            //Act

            createResenya.resenyar("",titulo, descipcion,rat,nombreC,apellido1C,apellido2C, lineas);

            //Assert

            Assert.True(createResenya.checkErrors(expectedError));

        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_FB_FA1_3()
        {
            //Arrange
            InitialStepsForResenyaBocadillo();

            //Act

            selectBocadillosParaResenyar.SearchBocadillos("", "0");

            //Assert

            Assert.True(selectBocadillosParaResenyar.checkHideCart());

        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_FB_FA2_4()
        {
            //Arrange
            InitialStepsForResenyaBocadillo();
            selectBocadillosParaResenyar.SearchBocadillos("", "0");
            selectBocadillosParaResenyar.addBocadillo(bocadilloNombre1);
            selectBocadillosParaResenyar.addBocadillo(bocadilloNombre2);

            var expectedBocadillos = new List<string[]> { new string[] { bocadilloNombre2, tipoPan2, bocadilloPVP2, tamanyo2, "Añadir" }, new string[] { "" }, };

            //Act

            selectBocadillosParaResenyar.removeBocadillo(bocadilloNombre1);

            //Assert

            Assert.True(selectBocadillosParaResenyar.CheckListOfBocadillos(expectedBocadillos));

        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_FB_FA4_5()
        {
            //Arrange
            InitialStepsForResenyaBocadillo();
            selectBocadillosParaResenyar.SearchBocadillos("", "0");
            selectBocadillosParaResenyar.addBocadillo(bocadilloNombre1);
            selectBocadillosParaResenyar.addBocadillo(bocadilloNombre2);

            selectBocadillosParaResenyar.continueToResenyar();

            var lineas = new List<List<string>> {
                new List<string> { bocadilloNombre2, "10" },
            };

            //Act

            createResenya.soloRellenar(nombreU, title, descripcion, rate, nombreC, apellido1C, apellido2C, lineas);
            createResenya.modificarBocadillo();
            selectBocadillosParaResenyar.removeBocadillo(bocadilloNombre1);
            selectBocadillosParaResenyar.continueToResenyar();
            

            //Assert

            Assert.True(createResenya.checkData(nombreU, title, descripcion, rate, nombreC, apellido1C, apellido2C, lineas));

        }

    }
}
