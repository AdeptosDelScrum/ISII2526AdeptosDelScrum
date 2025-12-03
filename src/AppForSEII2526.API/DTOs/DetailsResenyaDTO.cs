
namespace AppForSEII2526.API.DTOs
{
    public class DetailsResenyaDTO : ResenyaDTO
    {
        public DetailsResenyaDTO(int id, string nombreU, string titulo, string descripcion, DateTime fecha, int rate, List<LineasResenyaDTO> bocadillos, string nombre_cliente, string apellido1_cliente, string? apellido2_cliente)
            : base(id,nombreU,titulo,descripcion,rate,bocadillos,nombre_cliente,apellido1_cliente,apellido2_cliente)
        {
            this.fecha = fecha;
        }
        public DateTime fecha { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is DetailsResenyaDTO dTO &&
                   base.Name == dTO.Name &&
                   base.Title == dTO.Title &&
                   base.Description == dTO.Description &&
                   fecha == dTO.fecha &&
                   base.Rate == dTO.Rate &&
                   base.Lineas.SequenceEqual(dTO.Lineas);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.Name, base.Title, base.Description, fecha, base.Rate, base.Lineas);
        }
    }
}
