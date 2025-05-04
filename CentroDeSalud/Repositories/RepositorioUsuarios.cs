using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioUsuarios
    {
        Task<Guid> CrearUsuario(Usuario usuario);
        Task<Usuario> BuscarUsuarioPorId(Guid id);
        Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado);
        Task<Usuario> BuscarUsuarioPorNombre(string nombre);
        Task ActualizarRol(Guid usuarioId, int? rolId);
        Task ActualizarPassword(Usuario usuario);
    }

    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string _connectionString;

        public RepositorioUsuarios(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<Guid> CrearUsuario(Usuario usuario)
        {
            var id = Guid.NewGuid();
            usuario.Id = id;

            usuario.SecurityStamp = Guid.NewGuid().ToString();

            using var conexion = new SqlConnection(_connectionString);
            await conexion.ExecuteAsync(@"INSERT INTO Usuarios (Id, SecurityStamp, Email, EmailNormalizado, 
                         PasswordHash, Nombre, Apellidos, Telefono)
                         VALUES (@Id, @SecurityStamp, @Email, @EmailNormalizado, @PasswordHash, 
                         @Nombre, @Apellidos, @Telefono);", usuario);

            return id;
        }

        public async Task<Usuario> BuscarUsuarioPorId(Guid id)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QuerySingleOrDefaultAsync<Usuario>(@"Select * from Usuarios
                                                    Where Id = @Id", new { id });
        }

        public async Task<Usuario> BuscarUsuarioPorEmail(string emailNormalizado)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QuerySingleOrDefaultAsync<Usuario>(@"Select * from Usuarios
                                                    Where EmailNormalizado = @EmailNormalizado", new {emailNormalizado});
        }

        public async Task<Usuario> BuscarUsuarioPorNombre(string nombre)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QuerySingleOrDefaultAsync<Usuario>(@"Select * from Usuarios
                                                    Where Nombre = @Nombre", new { nombre });
        }

        public async Task ActualizarRol(Guid usuarioId, int? rolId)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.ExecuteAsync(@"Update Usuarios 
                                        SET RolId = @RolId 
                                        WHERE Id = @UsuarioId", new { RolId = rolId, UsuarioId = usuarioId });
        }

        public async Task ActualizarPassword(Usuario usuario)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.ExecuteAsync(@"Update Usuarios
                                          SET PasswordHash = @PasswordHash
                                          WHERE Id = @Id", usuario);
        }
    }
}
