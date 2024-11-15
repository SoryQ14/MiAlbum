﻿using album2.DTO.Fotos;
using album2.Entities;
using album2.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace album2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FotoController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment env; 

        public FotoController(IWebHostEnvironment env, ApplicationDbContext context)
        {
            this.env = env ?? throw new ArgumentNullException(nameof(env)); // Verifica si env es null 
            this.context = context;
        }

        [HttpPost("AgregarFoto")]
        public async Task<ActionResult> AgregarFoto([FromForm] RequestGuardarFoto fotoR)
        {
            var RutaF = string.Empty;
            Entities.Response respuesta;
            var usuarioExiste = await context.Usuarios.AnyAsync(u => u.Id == fotoR.userId);
            if (!usuarioExiste)
            {
                respuesta = new Entities.Response()
                {
                    CodResp = "404",
                    respuesta = "El usuario no existe en la base de datos"
                };
                return NotFound(respuesta);
            }
            if (fotoR.nombre.Length > 0 )
            {
                var nomSinEspacios = fotoR.nombre.Replace(" ", "_");
                var extensionFoto = Path.GetExtension(fotoR.foto.FileName.ToLower());//obtener la extension original de la foto
                if (extensionFoto != ".png" && extensionFoto != ".jpg" && extensionFoto != ".jpeg")
                {
                    extensionFoto = ".png"; 
                }

                var nomFoto = nomSinEspacios + extensionFoto;
                // Obtener la ruta física del wwwroot
                var rutaF = Path.Combine(env.WebRootPath, "imagenes", nomFoto); // Guardar en wwwroot/imagenes

                var existeFoto = await context.Fotos.AnyAsync(x => x.nombre == nomSinEspacios); 
                if (existeFoto)
                {
                    respuesta = new Entities.Response()
                    {
                        CodResp = "403",
                        respuesta = "Este archivo ya existe"
                    };
                    JsonConvert.SerializeObject(respuesta);
                    return BadRequest(respuesta);
                }
                Foto fotito = new Foto()
                {
                    nombre = nomSinEspacios,
                    descripcion = fotoR.descripcion,
                    ruta = Path.Combine("/imagenes", nomFoto),// Ruta relativa para almacenar en la base de datos
                    UsuarioId = fotoR.userId
                }; 
                using var stream = new FileStream(rutaF, FileMode.Create);
                await fotoR.foto.CopyToAsync(stream);
                context.Add(fotito);
                await context.SaveChangesAsync();
                
                respuesta = new Entities.Response()
                {
                    CodResp = "200",
                    respuesta = "Ya jalo tu"
                };
                //JsonConvert.SerializeObject(respuesta);
                return Ok(respuesta);
            }
            else
            {
                respuesta = new Entities.Response()
                {
                    CodResp = "403",
                    respuesta = "Ay drake, ahora sí se murio en serio :("
                };
                //JsonConvert.SerializeObject(respuesta);
                return BadRequest(respuesta);
            }
        }

        [HttpGet("VerFotosPorUsuario/{userId}")]
        public async Task<ActionResult> VerFotosPorUsuario(string userId)
        {
            Entities.Response resp;
            var usuarioExiste = await context.Usuarios.AnyAsync(u => u.Id == userId);
            if (!usuarioExiste)
            {
                resp = new Entities.Response()
                {
                    CodResp = "404",
                    respuesta = "El usuario no existe en la base de datos"
                };
                return NotFound(resp);
            }

            var fotos = await context.Fotos
                .Where(f => f.UsuarioId == userId)
                .Select(f => new
                {
                    Nombre = f.nombre,
                    Descripcion = f.descripcion
                })
                .ToListAsync();

            if (fotos.Count == 0)
            {
                resp = new Entities.Response()
                {
                    CodResp = "200",
                    respuesta = "Este usuario no tiene fotos almacenadas"
                };
                return Ok(resp);
            }
            return Ok(fotos);
        }

        [HttpGet("VerFotoPorNombre/{nombre}")]
        public async Task<IActionResult> VerFotoPorNombre(string nombre)
        {
            var foto = await context.Fotos.FirstOrDefaultAsync(f => f.nombre == nombre);
            if (foto == null)
            {
                var resp = new Entities.Response()
                {
                    CodResp = "404",
                    respuesta = "La foto no existe en la base de datos"
                };
                return NotFound(resp);
            }

            if (string.IsNullOrEmpty(foto.nombre))
            {
                var resp = new Entities.Response()
                {
                    CodResp = "400",
                    respuesta = "El nombre de la imagen es inválido"
                };
                return BadRequest(resp);
            }

            var extFoto = Path.GetExtension(foto.ruta);
            var rutaImg = Path.Combine(env.WebRootPath, "imagenes", foto.nombre + extFoto);

            if (!System.IO.File.Exists(rutaImg))
            {
                var resp = new Entities.Response()
                {
                    CodResp = "404",
                    respuesta = "El archivo de la imagen no se encuentra en el servidor"
                };
                return NotFound(resp);
            }

            var imgB = await System.IO.File.ReadAllBytesAsync(rutaImg);

            string mimeType = extFoto switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream" // Tipo MIME por defecto
            };

            return File(imgB, mimeType);
        }

        [HttpPut("ActualizarFoto/{fotoId}")]
        public async Task<IActionResult> ActualizarFoto(int fotoId,ActualizarFotoDTO fotoAct)
        {
            var foto = await context.Fotos.FirstOrDefaultAsync(f => f.Id == fotoId);
            if (foto == null)
            {
                return NotFound(new
                {
                    CodResp = "404",
                    respuesta = "La foto no existe en la base de datos"
                });
            }
            var extFoto = Path.GetExtension(foto.ruta);
            var nombreConEspaciosReemplazados = fotoAct.nombre?.Replace(" ", "_"); // Reemplazar espacios por guiones bajos
            var rutaImgActual = Path.Combine(env.WebRootPath, "imagenes", foto.nombre + extFoto);

            if (!string.IsNullOrEmpty(nombreConEspaciosReemplazados))
            {
                var rutaImgNueva = Path.Combine(env.WebRootPath, "imagenes", nombreConEspaciosReemplazados + extFoto);
                if (System.IO.File.Exists(rutaImgActual))
                {
                    System.IO.File.Move(rutaImgActual, rutaImgNueva);
                }
                foto.nombre = nombreConEspaciosReemplazados;
            }

            if (!string.IsNullOrEmpty(fotoAct.descripcion))
            {
                foto.descripcion = fotoAct.descripcion;
            }
           
            context.Fotos.Update(foto);
            await context.SaveChangesAsync();

            return Ok(new
            {
                CodResp = "200",
                respuesta = "La foto ha sido actualizada correctamente",
                FotoActualizada = foto
                
            });
        }

        [HttpDelete("EliminarFoto{id:int}")]
        public async Task<ActionResult> EliminarFoto(int id, IMapper mapper)
        {

            var foto = await context.Fotos.FirstOrDefaultAsync(x => x.Id == id); 
            if (foto == null)
            {
                return NotFound(new { mensaje = "La foto no existe en la base de datos" });
            }

            // Eliminar el archivo físico de la imagen 
            var rutaImg = Path.Combine(env.WebRootPath, "imagenes", foto.nombre + Path.GetExtension(foto.ruta));
            if (System.IO.File.Exists(rutaImg))
            {
                System.IO.File.Delete(rutaImg);
            }
            // Eliminar el registro de la foto de la base de datos
            context.Fotos.Remove(foto);
            await context.SaveChangesAsync();
            return Ok(new { mensaje = "Foto eliminada correctamente" });
        }


    }
}
