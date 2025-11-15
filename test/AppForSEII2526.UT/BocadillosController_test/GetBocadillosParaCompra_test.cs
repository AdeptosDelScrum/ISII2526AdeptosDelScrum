using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.BocadillosController_test
{
    public class GetBocadillosParaCompra_test: AppForSEII25264SqliteUT
    {
        public GetBocadillosParaCompra_test()
        {
            var tiposPan = new List<TipoPan>()
            {
                new TipoPan(0, "Normal"),
                new TipoPan(1, "Semillas"),
                new TipoPan(2, "Integral")
            };

            var bocadillos = new List<Bocadillo>()
            {
                new Bocadillo(0, "Completo",3, 50, Tamanyo.Normal, tiposPan[0]),
                new Bocadillo(1, "Politecnico", 4, 70, Tamanyo.Pequenyo, tiposPan[0]),
                new Bocadillo(2, "Americano", (float)2.5, 30, Tamanyo.Normal, tiposPan[0])
            };

            ApplicationUser user = new ApplicationUser(0, "Paco", "Olivares", "Alonso");

            var compra = new Compra(user.NombreCliente, user.Apellido1_Cliente, user.Apellido2_Cliente, DateTime.Today, 1, new Tarjeta(), new List<CompraBocadillo>());

            var compraBocadillo = new CompraBocadillo(bocadillos[0], 1, compra);

            _context.Add(user);
            _context.Add(tiposPan);
            _context.Add(bocadillos);
            _context.Add(compra);
            _context.SaveChanges();
        }
    }
}
