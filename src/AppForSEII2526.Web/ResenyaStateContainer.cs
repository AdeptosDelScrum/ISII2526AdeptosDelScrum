using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class ResenyaStateContainer
    {

        //we create an instance of Rental when an instance of RentalStateContainer is created
        public ResenyaDTO Resenya { get; private set; } = new ResenyaDTO()
        {
            Lineas = new List<LineasResenyaDTO>()
        };

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();



        public void AddBocadilloParaResenyar(LineasResenyaDTO bocadillo)
        {
            //before adding a movie we checked whether it has been already added
            if (!Resenya.Lineas.Any(ri => ri.Bocadillo.Id == bocadillo.Bocadillo.Id))
                //we add it if it is not in the list
                Resenya.Lineas.Add(new LineasResenyaDTO()
                {
                    Bocadillo = bocadillo.Bocadillo,
                    Puntuacion = bocadillo.Puntuacion,
                }
            );

        }

        //to delete movies from the list of selected movies
        public void RemoveResenyaItemParaResenyar(LineasResenyaDTO item)
        {
            Resenya.Lineas.Remove(item);

        }

        //we eliminate all the movies from the list
        public void ClearResenyaCart()
        {
            Resenya.Lineas.Clear();

        }

        //we have already finished the process of renting, thus, we create a new Rental 
        public void ResenyaProcessed()
        {
            //we have finished the rental process so we create a new object without data
            Resenya = new ResenyaDTO()
            {
                Lineas = new List<LineasResenyaDTO>()
            };
        }
    }
}
