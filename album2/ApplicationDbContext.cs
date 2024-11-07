using album2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace album2
{
    public class ApplicationDbContext : IdentityDbContext<Usuario>//para que agregue las prop extra del user
    {
        public ApplicationDbContext(DbContextOptions options) : base (options) 
        { 
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Configuracion de muchos a muchos
            builder.Entity<CategoriaFoto>()
                .HasKey(cf => new { cf.CategoriaId, cf.FotoId });

            builder.Entity<CategoriaFoto>()
                .HasOne(cf => cf.Categoria)
                .WithMany(c => c.categoriaFotos)
                .HasForeignKey(cf => cf.CategoriaId);

            builder.Entity<CategoriaFoto>()
                .HasOne(cf => cf.Foto)
                .WithMany(f => f.categoriaFotos)
                .HasForeignKey(cf => cf.FotoId); 
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Foto> Fotos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<CategoriaFoto> CategoriasFotos { get;set; }
    }
}
