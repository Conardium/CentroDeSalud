using CentroDeSalud.Models;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioRespuestasForos
    {
        Task<int> CrearRespuestaForo(RespuestaForo respuestaForo);
        Task<IEnumerable<RespuestaForo>> ListarRespuestasPorPreguntaId(int id);
    }

    public class ServicioRespuestasForos : IServicioRespuestasForos
    {
        private readonly IRepositorioRespuestasForos repositorioRespuestas;

        public ServicioRespuestasForos(IRepositorioRespuestasForos repositorioRespuestas)
        {
            this.repositorioRespuestas = repositorioRespuestas;
        }

        public async Task<int> CrearRespuestaForo(RespuestaForo respuestaForo)
        {
            return await repositorioRespuestas.CrearRespuestaForo(respuestaForo);
        }

        public async Task<IEnumerable<RespuestaForo>> ListarRespuestasPorPreguntaId(int id)
        {
            return await repositorioRespuestas.ListarRespuestasPorPreguntaId(id);
        }
    }
}
