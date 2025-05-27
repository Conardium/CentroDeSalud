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
        private readonly IServicioPreguntasForos servicioPreguntas;
        private readonly IServicioMedicos servicioMedicos;

        public ServicioRespuestasForos(IRepositorioRespuestasForos repositorioRespuestas, IServicioMedicos servicioMedicos, 
            IServicioPreguntasForos servicioPreguntas)
        {
            this.repositorioRespuestas = repositorioRespuestas;
            this.servicioMedicos = servicioMedicos;
            this.servicioPreguntas = servicioPreguntas;
        }

        public async Task<int> CrearRespuestaForo(RespuestaForo respuestaForo)
        {
            try
            {
                var resultado = await repositorioRespuestas.CrearRespuestaForo(respuestaForo);
                if(resultado != 0)
                {
                    await servicioPreguntas.ActualizarEstadoPregunta(respuestaForo.PreguntaForoId);
                    return resultado;
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        public async Task<IEnumerable<RespuestaForo>> ListarRespuestasPorPreguntaId(int id)
        {
            var respuestas = await repositorioRespuestas.ListarRespuestasPorPreguntaId(id);

            //Obtenemos los medicos de realizaron las respuestas
            foreach(var respuesta in respuestas)
            {
                respuesta.Medico = await servicioMedicos.ObtenerMedicoPorId(respuesta.MedicoId);
            }

            return respuestas;
        }
    }
}
