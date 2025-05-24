using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioRespuestasForos
    {
        Task<int> CrearRespuestaForo(RespuestaForo respuestaForo);
        Task<IEnumerable<RespuestaForo>> ListarRespuestasPorPreguntaId(int id);
    }

    public class RepositorioRespuestasForos : IRepositorioRespuestasForos
    {
        private readonly string _connectionString;

        public RepositorioRespuestasForos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<int> CrearRespuestaForo(RespuestaForo respuestaForo)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                var id = await conexion.QuerySingleAsync<int>(@"Insert into RespuestasForos (MedicoId, PreguntaForoId, Texto, FechaRespuesta)
                          Values (@MedicoId, @PreguntaForoId, @Texto, @FechaRespuesta);
                          select SCOPE_IDENTITY();", respuestaForo);
                return id;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<IEnumerable<RespuestaForo>> ListarRespuestasPorPreguntaId(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                return await conexion.QueryAsync<RespuestaForo>(@"Select * from RespuestasForos where PreguntaForoId = @PreguntaForoId",
                                            new { PreguntaForoId = id });
            }
            catch
            {
                return null;
            }
        }
    }
}
