using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioCitas
    {
        Task<int> CrearCita(Medico medico);
        Task<IEnumerable<Cita>> ObtenerCitasPendientesMedicoPorFecha(Guid medicoId, DateTime fecha);
    }

    public class RepositorioCitas : IRepositorioCitas
    {
        private readonly string _connectionString;

        public RepositorioCitas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<int> CrearCita (Medico medico)
        {
            using var conexion = new SqlConnection(_connectionString);

            //await conexion.ExecuteAsync(@"INSERT INTO Medicos (Id, Dni, Sexo, Especialidad)
              //             VALUES (@Id, @Dni, @Sexo, @Especialidad);", medico);

            return 0;
        }

        public async Task<IEnumerable<Cita>> ObtenerCitasPendientesMedicoPorFecha(Guid medicoId, DateTime fecha)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<Cita>(@"Select * from Citas where MedicoId = @MedicoId 
                                AND Fecha = @Fecha AND EstadoCita = 0", new { MedicoId = medicoId, Fecha = fecha});
        }
    }
}
