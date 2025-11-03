
namespace AppForSEII2526.API.DTOs
{
    public class DetailsLineasResenyaDTO
    {
        public DetailsLineasResenyaDTO()
        {
        }

        public DetailsLineasResenyaDTO(string nombre, float precio, Tamanyo tamanyo, int puntuacion)
        {
            this.nombre = nombre;
            this.precio = precio;
            this.tamanyo = tamanyo;
            this.puntuacion = puntuacion;
        }

        public string nombre {  get; set; }
        public float precio { get; set; }
        public Tamanyo tamanyo { get; set; }
        public int puntuacion {  get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DetailsLineasResenyaDTO dTO &&
                   nombre == dTO.nombre &&
                   precio == dTO.precio &&
                   tamanyo == dTO.tamanyo &&
                   puntuacion == dTO.puntuacion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(nombre, precio, tamanyo, puntuacion);
        }
    }
}
