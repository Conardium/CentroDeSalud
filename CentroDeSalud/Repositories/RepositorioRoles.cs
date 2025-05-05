using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioRoles
    {
        Task<Rol> BuscarRolPorId(int id);
        Task<Rol> BuscarRolPorNombre(string nombre);
        Task<bool> Existe(string nombre);
        Task InsertarRol(Rol rol);
        Task<IEnumerable<Rol>> ObtenerRoles();
    }

    public class RepositorioRoles : IRepositorioRoles
    {
        private readonly string _connectionString;

        public RepositorioRoles(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task InsertarRol(Rol rol)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(@"Insert into Roles (Id, Nombre, NombreNormalizado)
                                            Values (@Id, @Nombre, @NombreNormalizado)", rol);
        }

        public async Task<IEnumerable<Rol>> ObtenerRoles()
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<Rol>(@"Select * from Roles");
        }

        public async Task<Rol> BuscarRolPorId(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QuerySingleOrDefaultAsync<Rol>(@"Select * from Roles
                                            Where Id = @Id", new { id });
        }

        public async Task<Rol> BuscarRolPorNombre(string nombreNormalizado)
        {
            nombreNormalizado = nombreNormalizado.ToUpper();
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryFirstOrDefaultAsync<Rol>(@"Select * from Roles
                                Where NombreNormalizado = @NombreNormalizado", new { NombreNormalizado = nombreNormalizado });
        }

        public async Task<bool> Existe(string nombreNormalizado)
        {
            using var conexion = new SqlConnection(_connectionString);
            var existe = await conexion.QueryFirstOrDefaultAsync<int>(@"Select 1 from Roles
                                Where NombreNormalizado = @NombreNormalizado", new { NombreNormalizado = nombreNormalizado });

            return existe == 1;
        }
    }
}
