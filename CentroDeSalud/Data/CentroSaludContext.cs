using Microsoft.EntityFrameworkCore;
using CentroDeSalud.Models;
using static Dapper.SqlMapper;

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
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Mensaje> Mensajes { get; set; }
        public DbSet<Informe> Informes { get; set; }
        public DbSet<PreguntaForo> PreguntasForos { get; set; }
        public DbSet<RespuestaForo> RespuestasForos { get; set; }
        public DbSet<Publicacion> Publicaciones { get; set; }

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
            modelBuilder.Entity<Chat>().ToTable("Chats");
            modelBuilder.Entity<Mensaje>().ToTable("Mensajes");
            modelBuilder.Entity<Informe>().ToTable("Informes");
            modelBuilder.Entity<PreguntaForo>().ToTable("PreguntasForos");
            modelBuilder.Entity<RespuestaForo>().ToTable("RespuestasForos");
            modelBuilder.Entity<Publicacion>().ToTable("Publicaciones");

            //========================== RELACIONES ===============================
            //===> USUARIO
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId)
                .OnDelete(DeleteBehavior.SetNull);

            //===> LOGIN EXTERNO
            modelBuilder.Entity<UsuarioLoginExterno>(entity =>
            {
                //Indice unico para evitar duplicados por proveedor+clave
                entity.HasIndex(e => new { e.LoginProvider, e.ProviderKey }).IsUnique();

                //FKs
                entity.HasOne<Usuario>().WithMany(u => u.LoginsExternos).HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            //===> CITA
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

            //===> CHAT
            modelBuilder.Entity<Chat>().HasIndex(c => new { c.PacienteId, c.MedicoId }).IsUnique();

            modelBuilder.Entity<Chat>()
                .HasOne(p => p.Paciente)
                .WithMany(c => c.Chats)
                .HasForeignKey(c => c.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Chat>()
                .HasOne(m => m.Medico)
                .WithMany(c => c.Chats)
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            //===> MENSAJE
            modelBuilder.Entity<Mensaje>()
                .HasOne(u => u.UsuarioRemitente)
                .WithMany(m => m.MensajesEnviados)
                .HasForeignKey(m => m.RemitenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mensaje>()
                .HasOne(u => u.UsuarioReceptor)
                .WithMany(m => m.MensajesRecibidos)
                .HasForeignKey(m => m.ReceptorId)
                .OnDelete(DeleteBehavior.Restrict);

            //====> INFORME
            modelBuilder.Entity<Informe>()
                .HasOne(p => p.Paciente)
                .WithMany(i => i.InformesPaciente)
                .HasForeignKey(i => i.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Informe>()
                .HasOne(m => m.Medico)
                .WithMany(i => i.InformesMedico)
                .HasForeignKey(i => i.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            //====> PUBLICACION
            modelBuilder.Entity<Publicacion>().HasIndex(p => new { p.Slug }).IsUnique();
        }
    }
}
