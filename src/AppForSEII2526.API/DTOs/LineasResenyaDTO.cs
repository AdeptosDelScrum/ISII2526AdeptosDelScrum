
namespace AppForSEII2526.API.DTOs
{
    public class LineasResenyaDTO
    {
        public LineasResenyaDTO(BocadilloDTO bocadillo, int puntuacion)
        {
            this.bocadillo = bocadillo;
            Puntuacion = puntuacion;
        }

        public BocadilloDTO bocadillo { get; set; }
        public int Puntuacion { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is LineasResenyaDTO dTO &&
                   EqualityComparer<BocadilloDTO>.Default.Equals(bocadillo, dTO.bocadillo) &&
                   Puntuacion == dTO.Puntuacion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(bocadillo, Puntuacion);
        }
    }
}
