using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioPacientes
    {
        Task<Guid> CrearPaciente(Paciente paciente);
        Task<Paciente> ObtenerPacientePorId(Guid id);
    }

    public class RepositorioPacientes : IRepositorioPacientes
    {
        private readonly string _connectionString;

        public RepositorioPacientes(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<Guid> CrearPaciente(Paciente paciente)
        {
            using var conexion = new SqlConnection(_connectionString);

            await conexion.ExecuteAsync(@"INSERT INTO Pacientes (Id, Dni, FechaNacimiento, GrupoSanguineo, Direccion, Sexo)
                               VALUES (@Id, @Dni, @FechaNacimiento, @GrupoSanguineo, @Direccion, @Sexo);", paciente);

            return paciente.Id;
        }

        public async Task<Paciente> ObtenerPacientePorId(Guid id)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryFirstOrDefaultAsync<Paciente>(@"Select * from Pacientes where Id = @Id", new {Id = id});
        }
    }
}
