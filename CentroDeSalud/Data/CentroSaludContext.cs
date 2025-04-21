using Microsoft.EntityFrameworkCore;
using CentroDeSalud.Models;

namespace CentroDeSalud.Data
{
    public class CentroSaludContext : DbContext
    {
        public CentroSaludContext(DbContextOptions<CentroSaludContext> options) 
            : base(options) { }
        
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Rol> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>().ToTable("Pacientes");
            modelBuilder.Entity<Medico>().ToTable("Medicos");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Rol>().ToTable("Roles");

            //Relaciones
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
