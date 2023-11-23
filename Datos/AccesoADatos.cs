using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace MotoAmigoApiBD.Datos
{
    public class AccesoADatos : DbContext
    {
        public AccesoADatos(DbContextOptions<AccesoADatos> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Productos> Productos { get; set; }
        public DbSet<ProductoUsuarioDestacado> ProductoUsuarioDestacado { get; set; }
        public DbSet<ProductoUsuarioDenunciado> ProductoUsuarioDenunciado { get; set; }
        public DbSet<Mail> Mail { get; set; }
    }
}
