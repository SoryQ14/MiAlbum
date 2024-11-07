using Microsoft.AspNetCore.Identity;

namespace album2.Models
{
    public class Usuario : IdentityUser
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string  ApellidoMaterno { get; set; }

        public ICollection<Foto> Fotos { get; set; } = new List<Foto>();
    }
}
