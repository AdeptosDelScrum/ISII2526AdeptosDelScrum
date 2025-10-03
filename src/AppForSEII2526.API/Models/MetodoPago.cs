namespace AppForSEII2526.API.Models
{
    public abstract class MetodoPago
    {
        protected MetodoPago() { }
        public abstract int Id { get; protected set; }
        public abstract String Metodo {  get; }
    }
}
