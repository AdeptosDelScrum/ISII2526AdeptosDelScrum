
namespace AppForSEII2526.API.DTOs
{
    public class ResenyaDTO
    {
        public ResenyaDTO(int id, string? name, string title, string description, int rate, IList<LineasResenyaDTO> lineas, string nombre_cliente, string apellido1_cliente, string? apellido2_cliente)
        {
            Id = id;
            Name = name;
            Title = title;
            Description = description;
            Rate = rate;
            Lineas = lineas;
            Nombre_cliente = nombre_cliente ?? throw new ArgumentNullException(nameof(nombre_cliente)); 
            Apellido1_cliente = apellido1_cliente ?? throw new ArgumentNullException(nameof(apellido1_cliente)); ;
            Apellido2_cliente = apellido2_cliente;
        }

        public int Id { get; set; }
        public string Nombre_cliente { get; set; }

        public string Apellido1_cliente { get; set; }
        public string? Apellido2_cliente { get; set; }
        public string? Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
        public IList<LineasResenyaDTO> Lineas { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ResenyaDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   Title == dTO.Title &&
                   Description == dTO.Description &&
                   Rate == dTO.Rate &&
                   EqualityComparer<IList<LineasResenyaDTO>>.Default.Equals(Lineas, dTO.Lineas);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Title, Description, Rate, Lineas);
        }
    }
}
