using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
        public class CompraStateContainer
        {

            //we create an instance of Rental when an instance of RentalStateContainer is created
            public CompraBocadilloForCreateDTO Compra { get; private set; } = new CompraBocadilloForCreateDTO()
            {
                BocadillosComprados = new List<CompraBocadilloItemDTO>()
            };

            //we compute the TotalPrice of the movies we have selected for renting them
            public decimal TotalPrice
            {
                get
                {
                    return Convert.ToDecimal(Compra.BocadillosComprados.Sum(ci => ci.Precio * ci.Cantidad));
                }
            }

            public event Action? OnChange;

            private void NotifyStateChanged() => OnChange?.Invoke();



            public void AddBocadilloParaCompra(BocadilloDTO bocadillo)
            {
                if (!Compra.BocadillosComprados.Any(b => b.Nombre == bocadillo.Name))
                {
                    Compra.BocadillosComprados.Add(
                        new CompraBocadilloItemDTO
                        {
                            Nombre = bocadillo.Name,
                            Precio = (float)bocadillo.Pvp,
                            TipoPan = bocadillo.TipoPan,
                            Cantidad = 1
                        }
                    );
                }

                NotifyStateChanged();
            }

            //to delete movies from the list of selected movies
            public void RemoveItemParaCompra(CompraBocadilloItemDTO item)
            {
                Compra.BocadillosComprados.Remove(item);

            }

            //we eliminate all the movies from the list
            public void ClearCompraCart()
            {
                Compra.BocadillosComprados.Clear();

            }

            //we have already finished the process of renting, thus, we create a new Rental 
            public void RentalProcessed()
            {
                //we have finished the rental process so we create a new object without data
                Compra = new CompraBocadilloForCreateDTO()
                {
                    BocadillosComprados = new List<CompraBocadilloItemDTO>()
                };
            }
        }
    }