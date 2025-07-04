﻿using CentroDeSalud.Enumerations;
using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioDisponibilidadesMedicos
    {
        Task<bool> CrearDisponibilidad(DisponibilidadMedico modelo);
        Task<DisponibilidadMedico> DisponibilidadPorMedicoIdyDiaSemana(Guid medicoId, int diaSemana);
        Task<IEnumerable<DisponibilidadMedico>> ObtenerHorarioMedico(Guid medicoId);
    }

    public class RepositorioDisponibilidadesMedicos : IRepositorioDisponibilidadesMedicos
    {
        private readonly string _connectionString;

        public RepositorioDisponibilidadesMedicos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<bool> CrearDisponibilidad(DisponibilidadMedico modelo)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                await conexion.ExecuteAsync(@"Insert into DisponibilidadesMedicos (MedicoId, DiaSemana, 
                    HoraInicio, HoraFin)
                    Values (@MedicoId, @DiaSemana, @HoraInicio, @HoraFin)", modelo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DisponibilidadMedico> DisponibilidadPorMedicoIdyDiaSemana(Guid medicoId, int diaSemana)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryFirstOrDefaultAsync<DisponibilidadMedico>(@"Select * from DisponibilidadesMedicos
                                        Where MedicoId = @MedicoId AND DiaSemana = @DiaSemana", 
                                        new { MedicoId = medicoId, DiaSemana = diaSemana });
        }

        public async Task<IEnumerable<DisponibilidadMedico>> ObtenerHorarioMedico(Guid medicoId)
        {
            using var conexion = new SqlConnection(_connectionString);
            return await conexion.QueryAsync<DisponibilidadMedico>(@"Select DiaSemana, HoraInicio, HoraFin 
                            from DisponibilidadesMedicos where MedicoId = @MedicoId order by DiaSemana", new { MedicoId = medicoId});
        }
    }
}
