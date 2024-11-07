using System.ComponentModel.DataAnnotations;

namespace album2.DTO
{
    public class UsuarioDTO
    {
        public string Nombre { get; set; }
        public string ApellidoPaterno {  get; set; }
        public string ApellidoMaterno { get; set;}
        [EmailAddress]
        public string Email { get; set; }   
        public string password {  get; set; }
    }
}
