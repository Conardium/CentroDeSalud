using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioPreguntasForo
    {
        Task<PreguntaForo> BuscarPreguntaPorId(int id);
        Task<int> CrearPreguntaForo(PreguntaForo preguntaForo);
        Task<IEnumerable<PreguntaForo>> ListarPreguntasForo();
    }

    public class RepositorioPreguntasForo : IRepositorioPreguntasForo
    {
        private readonly string _connectionString;

        public RepositorioPreguntasForo(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<int> CrearPreguntaForo(PreguntaForo preguntaForo)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                var id = await conexion.QuerySingleAsync<int>(@"Insert into PreguntasForos (PacienteId, Titulo, Texto,
                          FechaCreacion, EstadoPregunta)
                          Values (@PacienteId, @Titulo, @Texto, @FechaCreacion, @EstadoPregunta);
                          select SCOPE_IDENTITY();", preguntaForo);
                return id;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<PreguntaForo> BuscarPreguntaPorId(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                return await conexion.QueryFirstOrDefaultAsync<PreguntaForo>(@"Select * from PreguntasForos where Id = @Id",
                                            new { Id = id });
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<PreguntaForo>> ListarPreguntasForo()
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                return await conexion.QueryAsync<PreguntaForo>(@"Select * from PreguntasForos");
            }
            catch
            {
                return null;
            }
        }
    }
}
