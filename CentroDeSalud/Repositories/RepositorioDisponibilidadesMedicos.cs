using CentroDeSalud.Enumerations;
using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioDisponibilidadesMedicos
    {
        Task<DisponibilidadMedico> DisponibilidadPorMedicoIdyDiaSemana(Guid medicoId, int diaSemana);
    }

    public class RepositorioDisponibilidadesMedicos : IRepositorioDisponibilidadesMedicos
    {
        private readonly string _connectionString;

        public RepositorioDisponibilidadesMedicos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<DisponibilidadMedico> DisponibilidadPorMedicoIdyDiaSemana(Guid medicoId, int diaSemana)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryFirstOrDefaultAsync<DisponibilidadMedico>(@"Select * from DisponibilidadesMedicos
                                        Where MedicoId = @MedicoId AND DiaSemana = @DiaSemana", 
                                        new { MedicoId = medicoId, DiaSemana = diaSemana });
        }
    }
}
