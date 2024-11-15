using album2.Models;
using AutoMapper;
using login.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace album2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaFotoController : Controller
    {
        private readonly ApplicationDbContext context;

        public CategoriaFotoController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
        }

        [HttpPost("AgregarCatFoto")]
        public async Task<ActionResult> AgregarCatFoto(int catId, int fotoId)
        {
            var catFoto = new CategoriaFoto { CategoriaId = catId, FotoId = fotoId };
            context.CategoriasFotos.Add(catFoto);
            await context.SaveChangesAsync();
            return Ok(catFoto);
        }

        [HttpGet("ListarCatFotos")]
        public async Task<ActionResult> ListarCatFotos()
        {
            var relacion = await context.CategoriasFotos
                .Include(cf => cf.Categoria)
                .Include(cf => cf.Foto)
                .ToListAsync();

            return Ok(relacion.Select(r => new
            {
                Categoria = r.Categoria.nombre,
                Foto = r.Foto.nombre
            })); 
        }

        [HttpDelete("EliminarCatFoto")]
        public async Task<ActionResult> EliminarCatFoto(int catId, int fotoId)
        {
            var categoriaFoto = await context.CategoriasFotos.FirstOrDefaultAsync(cf => cf.CategoriaId == catId && cf.FotoId == fotoId);
            if (categoriaFoto == null) return NotFound(new { mensaje = "La relación no existe" }); 

            context.CategoriasFotos.Remove(categoriaFoto);
            await context.SaveChangesAsync();

            return Ok(new { mensaje = "Relación eliminada correctamente" }); 
        }
    }
}
