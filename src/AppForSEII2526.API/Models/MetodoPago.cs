namespace AppForSEII2526.API.Models
{
    public abstract class MetodoPago
    {
        public MetodoPago() { }
        public MetodoPago(int id)
        {
            Id = id;
        }
        public int Id { get; set; }
    }
}
