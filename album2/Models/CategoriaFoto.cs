namespace album2.Models
{
    public class CategoriaFoto
    {
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
        public int FotoId { get; set; }
        public Foto Foto { get; set; }
        
    }
}
