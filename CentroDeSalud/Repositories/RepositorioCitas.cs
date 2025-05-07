using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioCitas
    {
        Task<Cita> BuscarPorFechaHora(DateTime fecha, TimeSpan hora);
        Task<int> CrearCita(Cita cita);
        Task<IEnumerable<Cita>> ObtenerCitasPendientesMedicoPorFecha(Guid medicoId, DateTime fecha);
    }

    public class RepositorioCitas : IRepositorioCitas
    {
        private readonly string _connectionString;

        public RepositorioCitas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<int> CrearCita (Cita cita)
        {
            using var conexion = new SqlConnection(_connectionString);

            var id = await conexion.QuerySingleAsync<int>(@"Insert into Citas (PacienteId, MedicoId, Fecha, Hora, EstadoCita, Motivo)
                                values (@PacienteId, @MedicoId, @Fecha, @Hora, @EstadoCita, @Motivo);
                                select SCOPE_IDENTITY();", cita);

            return id;
        }

        public async Task<IEnumerable<Cita>> ObtenerCitasPendientesMedicoPorFecha(Guid medicoId, DateTime fecha)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<Cita>(@"Select * from Citas where MedicoId = @MedicoId 
                                AND Fecha = @Fecha AND EstadoCita = 0", new { MedicoId = medicoId, Fecha = fecha});
        }

        public async Task<Cita> BuscarPorFechaHora(DateTime fecha, TimeSpan hora)
        {
            var fechaSolo = fecha.Date;
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryFirstOrDefaultAsync<Cita>(@"Select * from Citas 
                            where Fecha = @Fecha AND Hora = @Hora", new {Fecha = fechaSolo, Hora = hora});
        }
    }
}
