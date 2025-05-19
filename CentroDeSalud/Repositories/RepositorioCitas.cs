using CentroDeSalud.Data;
using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioCitas
    {
        Task<Cita> BuscarCitaPorId(int id);
        Task<Cita> BuscarPorFechaHora(DateTime fecha, TimeSpan hora);
        Task<bool> CancelarCita(int id);
        Task<int> CrearCita(Cita cita);
        Task<bool> EliminarCita(int id);
        Task<IEnumerable<Cita>> ListarCitasPorUsuario(Guid id, string rol);
        Task<IEnumerable<Cita>> ObtenerCitasPendientesMedicoPorFecha(Guid medicoId, DateTime fecha);
        Task<IEnumerable<Cita>> ObtenerCitasPendientesPorIdUsuario(Guid idUsuario, string rol);
    }

    public class RepositorioCitas : IRepositorioCitas
    {
        private readonly string _connectionString;

        public RepositorioCitas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<Cita> BuscarCitaPorId(int id)
        {
            using var conexion = new SqlConnection(_connectionString);

            return await conexion.QueryFirstOrDefaultAsync<Cita>(@"Select * from Citas where Id = @Id", new {Id = id});
        }

        public async Task<int> CrearCita (Cita cita)
        {
            using var conexion = new SqlConnection(_connectionString);

            var id = await conexion.QuerySingleAsync<int>(@"Insert into Citas (PacienteId, MedicoId, Fecha, Hora, EstadoCita, Motivo)
                                values (@PacienteId, @MedicoId, @Fecha, @Hora, @EstadoCita, @Motivo);
                                select SCOPE_IDENTITY();", cita);

            return id;
        }

        public async Task<bool> EliminarCita(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                await conexion.ExecuteAsync(@"Delete from Citas where Id = @Id", new {Id = id});
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Cita>> ListarCitasPorUsuario(Guid id, string rol)
        {
            using var conexion = new SqlConnection(_connectionString);
            if (rol == Constantes.RolPaciente)
                return await conexion.QueryAsync<Cita>(@"Select * from Citas Where PacienteId = @PacienteId",
                                                        new { PacienteId = id });
            else if (rol == Constantes.RolMedico)
                return await conexion.QueryAsync<Cita>(@"Select * from Citas Where MedicoId = @MedicoId",
                                                        new { MedicoId = id });
            else
                return null;
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

        public async Task<IEnumerable<Cita>> ObtenerCitasPendientesPorIdUsuario(Guid idUsuario, string rol)
        {
            using var conexion = new SqlConnection(_connectionString);
            if (rol == Constantes.RolPaciente)
                return await conexion.QueryAsync<Cita>(@"Select * from Citas Where PacienteId = @PacienteId AND EstadoCita = 0",
                                                        new { PacienteId = idUsuario });
            else if (rol == Constantes.RolMedico)
                return await conexion.QueryAsync<Cita>(@"Select * from Citas Where MedicoId = @MedicoId AND EstadoCita = 0",
                                                        new { MedicoId = idUsuario });
            else
                return null;
        }

        public async Task<bool> CancelarCita(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            await conexion.ExecuteAsync(@"Update Citas Set EstadoCita = 2 where Id = @Id", new {Id = id});
            return true;
        }
    }
}
