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
        public DbSet<UsuarioLoginExterno> UsuariosLoginExterno { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<DisponibilidadMedico> DisponibilidadesMedicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>().ToTable("Pacientes");
            modelBuilder.Entity<Medico>().ToTable("Medicos");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Rol>().ToTable("Roles");
            modelBuilder.Entity<UsuarioLoginExterno>().ToTable("UsuariosLoginExterno");
            modelBuilder.Entity<Cita>().ToTable("Citas");
            modelBuilder.Entity<DisponibilidadMedico>().ToTable("DisponibilidadesMedicos");

            //========================== RELACIONES ===============================
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UsuarioLoginExterno>(entity =>
            {
                //Indice unico para evitar duplicados por proveedor+clave
                entity.HasIndex(e => new { e.LoginProvider, e.ProviderKey }).IsUnique();

                //Clave foranea
                entity.HasOne<Usuario>().WithMany(u => u.LoginsExternos).HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Cita>()
                .HasOne(p => p.Paciente)
                .WithMany(c => c.Citas)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasOne(m => m.Medico)
                .WithMany(c => c.Citas)
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cita>()
                .HasIndex(c => new { c.Fecha, c.Hora })
                .IsUnique();
        }
    }
}
