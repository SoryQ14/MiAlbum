namespace album2.Models
{
    public class Foto
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public string ruta { get; set; }

        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }//propiedad de navegacion

        public ICollection<CategoriaFoto> categoriaFotos { get; set; } = new List<CategoriaFoto>();
    }
}
