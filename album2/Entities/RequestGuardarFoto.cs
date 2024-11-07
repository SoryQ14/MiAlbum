namespace album2.Entities
{
    public class RequestGuardarFoto
    {
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string userId { get; set; }
        public IFormFile foto { get; set; }
    }
}
