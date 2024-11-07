using album2.DTO;
using album2.DTO.Fotos;
using album2.Models;
using AutoMapper;

namespace album2
{
    public class AutoMapper : Profile
    {
        public AutoMapper() 
        { 
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();
            CreateMap<Foto, ActualizarFotoDTO>().ReverseMap();
            CreateMap<Foto, EliminarFotoDTO>().ReverseMap(); 
        }
    }
}
