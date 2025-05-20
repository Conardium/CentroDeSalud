using CentroDeSalud.Models;
using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioInformes
    {
        Task<int> CrearInforme(Informe informe);
        Task<bool> EditarInforme(Informe informe);
        Task<IEnumerable<Informe>> ListarInformes();
        Task<IEnumerable<Informe>> ListarInformesDelMedico(Guid medicoId);
        Task<IEnumerable<Informe>> ListarInformesDelPaciente(Guid pacienteId);
        Task<Informe> ObtenerInformePorId(int idInforme);
    }

    public class RepositorioInformes : IRepositorioInformes
    {
        private readonly string _connectionString;

        public RepositorioInformes(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<int> CrearInforme(Informe informe)
        {
            using var conexion = new SqlConnection(_connectionString);
            var id = await conexion.QuerySingleAsync<int>(@"Insert into Informes (FechaCreacion, FechaModificacion, EstadoInforme,
                    Diagnostico, Tratamiento, Notas, Recomendaciones, ArchivosAdjuntos, PacienteId, MedicoId)
                 Values (@FechaCreacion, @FechaModificacion, @EstadoInforme, @Diagnostico, @Tratamiento, @Notas, @Recomendaciones, 
                    @ArchivosAdjuntos, @PacienteId, @MedicoId);
                 select SCOPE_IDENTITY();", informe);
            return id;
        }

        public async Task<bool> EditarInforme(Informe informe)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                await conexion.ExecuteAsync(@"Update Informes SET FechaModificacion = @FechaModificacion, 
                    EstadoInforme = @EstadoInforme, Diagnostico = @Diagnostico, Tratamiento = @Tratamiento,
                    Notas = @Notas, Recomendaciones = @Recomendaciones, ArchivosAdjuntos = @ArchivosAdjuntos
                    Where Id = @Id", informe);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Informe>> ListarInformes()
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<Informe>(@"Select * from Informes");
        }

        public async Task<IEnumerable<Informe>> ListarInformesDelPaciente(Guid pacienteId)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                return await conexion.QueryAsync<Informe>(@"Select * from Informes Where PacienteId = @PacienteId",
                    new {PacienteId = pacienteId});
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<Informe>> ListarInformesDelMedico(Guid medicoId)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                return await conexion.QueryAsync<Informe>(@"Select * from Informes Where MedicoId = @MedicoId",
                    new { MedicoId = medicoId });
            }
            catch
            {
                return null;
            }
        }

        public async Task<Informe> ObtenerInformePorId(int idInforme)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryFirstOrDefaultAsync<Informe>(@"Select * from Informes where Id = @Id",
                new {Id = idInforme});
        }
    }
}
