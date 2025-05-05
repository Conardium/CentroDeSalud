using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioMedicos
    {
        Task<Guid> CrearMedico(Medico medico);
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
    }
}
