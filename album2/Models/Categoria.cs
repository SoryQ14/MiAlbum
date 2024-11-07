namespace album2.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string nombre { get; set; }

        public ICollection<CategoriaFoto> categoriaFotos { get; set; } = new List<CategoriaFoto>();
    }
}
