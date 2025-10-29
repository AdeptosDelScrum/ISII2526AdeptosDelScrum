namespace AppForSEII2526.API.DTOs
{
    public class ResenyaDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
        public IList<LineasResenyaDTO> Lineas { get; set; }

    }
}
