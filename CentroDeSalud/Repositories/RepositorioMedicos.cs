using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioMedicos
    {
        Task<Guid> CrearMedico(Medico medico);
        Task<IEnumerable<Medico>> ListadoMedicos();
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

        public async Task<IEnumerable<Medico>> ListadoMedicos()
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<Medico>(@"Select u.Id, u.Nombre, u.Apellidos, m.Especialidad from Usuarios u 
                                                        inner join Medicos m on u.Id = m.Id where RolId = 2");
        }
    }
}
