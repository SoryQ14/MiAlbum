using album2.DTO;
using album2.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace album2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : Controller
    {
        private readonly ApplicationDbContext context; 
        private readonly IMapper mapper;
        public CategoriaController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost("AgregarCategoria")]
        public async Task<ActionResult> AgregarCategoria(CategoriaDTO categoriaDTO)
        {
            var catExiste = await context.Categorias.AnyAsync(x => x.nombre == categoriaDTO.nombre);
            if (catExiste)
            {
                return BadRequest($"Categoria {categoriaDTO.nombre} duplicada");
            }
            var categoria = mapper.Map<Categoria>(categoriaDTO);

            context.Add(categoria);
            await context.SaveChangesAsync();
            return Ok(categoria);
        }

        [HttpGet("ListarCategoria")]
        public async Task<ActionResult<List<Categoria>>> ListarCategoria()
        {
            var categoria = await context.Categorias.ToListAsync(); 
            var catDTO = mapper.Map<List<Categoria>>(categoria);
            return Ok(catDTO);
        }

        [HttpGet("{id:int}", Name = "Obtener categoria")]
        public async Task<ActionResult<Categoria>> GetCategoria (int id)
        {
            var cat = await context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
            if(cat == null)
                return NotFound();      
            var categoriaDTO = mapper.Map<Categoria>(cat);
            return Ok(categoriaDTO);
        }

        [HttpPut("ActualizarCat{id:int}")]
        public async Task<ActionResult> ActualizarCat(int id, CategoriaDTO categoriaDTO)
        {
            var catExiste = await context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
            var categorias = mapper.Map<Categoria>(catExiste);
            if (catExiste == null)
                return NotFound(); 

            catExiste.nombre = categoriaDTO.nombre;

            context.Update(categorias);
            await context.SaveChangesAsync();
            return Ok(categorias); 
        }

        [HttpDelete("EliminarCat{id:int}")]
        public async Task<ActionResult> EliminarCat(int id)
        {
            var catExiste = await context.Categorias.FirstOrDefaultAsync(x => x.Id == id);
            var catDTO = mapper.Map<Categoria>(catExiste);
            if (catExiste == null)
                return NotFound();

            context.Remove(catDTO);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
