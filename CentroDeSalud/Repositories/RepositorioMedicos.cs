using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioMedicos
    {
        Task<bool> ActualizarDatosPerfil(EditarPerfilViewModel modelo);
        Task<Guid> CrearMedico(Medico medico);
        Task<IEnumerable<Medico>> ListadoMedicos();
        Task<Medico> ObtenerMedicoPorId(Guid id);
    }

    public class RepositorioMedicos : IRepositorioMedicos
    {
        private readonly string _connectionString;

        public RepositorioMedicos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<Guid> CrearMedico(Medico medico)
        {
            using var conexion = new SqlConnection(_connectionString);

            await conexion.ExecuteAsync(@"INSERT INTO Medicos (Id, Dni, Sexo, Especialidad)
                           VALUES (@Id, @Dni, @Sexo, @Especialidad);", medico);

            return medico.Id;
        }

        public async Task<Medico> ObtenerMedicoPorId(Guid id)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryFirstOrDefaultAsync<Medico>(@"Select * from Medicos where Id = @Id", new { Id = id });
        }

        public async Task<IEnumerable<Medico>> ListadoMedicos()
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<Medico>(@"Select u.Id, u.Nombre, u.Apellidos, m.Especialidad from Usuarios u 
                                                        inner join Medicos m on u.Id = m.Id where RolId = 2");
        }

        public async Task<bool> ActualizarDatosPerfil(EditarPerfilViewModel modelo)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                await conexion.ExecuteAsync(@"Update Usuarios 
                        SET Nombre = @Nombre, Apellidos = @Apellidos, Telefono = @Telefono Where Id = @Id", modelo);

                await conexion.ExecuteAsync(@"Update Medicos 
                        SET Sexo = @Sexo, Especialidad = @Especialidad Where Id = @Id", modelo);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
