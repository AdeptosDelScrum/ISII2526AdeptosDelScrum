
namespace AppForSEII2526.API.DTOs
{
    public class DetailsResenyaDTO
    {
        public DetailsResenyaDTO(string nombreU, string titulo, string descripcion, DateTime fecha, int rate, List<DetailsLineasResenyaDTO> bocadillos)
        {
            this.nombreU = nombreU;
            this.titulo = titulo;
            this.descripcion = descripcion;
            this.fecha = fecha;
            this.rate = rate;
            this.bocadillos = bocadillos;
        }

        public string nombreU {  get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        public DateTime fecha { get; set; }
        public int rate { get; set; }
        public List<DetailsLineasResenyaDTO> bocadillos { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DetailsResenyaDTO dTO &&
                   nombreU == dTO.nombreU &&
                   titulo == dTO.titulo &&
                   descripcion == dTO.descripcion &&
                   fecha == dTO.fecha &&
                   rate == dTO.rate &&
                   EqualityComparer<IList<DetailsLineasResenyaDTO>>.Default.Equals(bocadillos, dTO.bocadillos);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(nombreU, titulo, descripcion, fecha, rate, bocadillos);
        }
    }
}
