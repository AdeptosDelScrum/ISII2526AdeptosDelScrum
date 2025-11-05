using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppForSEII2526.UT.BocadillosController_test
{
    public class GetBocadillosParaResenya_test : AppForSEII25264SqliteUT
    {
        public GetBocadillosParaResenya_test()
        {
            var tipopan = new TipoPan();
            tipopan.PanId = 1;
            tipopan.Nombre = "sandwitch";
            var bocadillo = new Bocadillo();
            bocadillo.Id = 1;
            bocadillo.Nombre = "Focata";
            bocadillo.PVP = 10;
            bocadillo.Stock = 3;
            bocadillo.TamanyoBocadillo = Tamanyo.Normal;
            bocadillo.TipoPan = tipopan;
            _context.AddRange(tipopan);
            _context.AddRange(bocadillo);
            _context.SaveChanges();
        }


        [Fact]
        public async Task bocadillosFiltrosNULL()
        {
            //Arrange
            var tipopan_esperado = new TipoPan();
            tipopan_esperado.PanId = 1;
            tipopan_esperado.Nombre = "sandwitch";

            var DTOs = new BocadilloDTO("Focata", Tamanyo.Normal, "sandwitch", 10);
            var resultado_esperado = new List<BocadilloDTO>();
            resultado_esperado.Add(DTOs);

            var mock = new Mock<ILogger<BocadillosController>>();
            ILogger<BocadillosController> logger = mock.Object;
            BocadillosController controller = new BocadillosController(_context, logger);


            //Act

            var result = await controller.GetBocadillosParaResenya(null, null);


            //Assert

            var okresult = Assert.IsType<OkObjectResult>(result);

            var resenyaresult = Assert.IsType<List<BocadilloDTO>>(okresult.Value);
            Assert.Equal(resultado_esperado, resenyaresult);

        }
    }
}
