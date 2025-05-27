using CentroDeSalud.Models;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioPreguntasForos
    {
        Task<bool> ActualizarEstadoPregunta(int id);
        Task<int> CrearPreguntaForo(PreguntaForo preguntaForo);
        Task<IEnumerable<PreguntaForo>> ListarCincoUltimasPreguntas();
        Task<IEnumerable<PreguntaForo>> ListarPreguntasForo();
        Task<PreguntaForo> ObtenerPreguntaPorId(int id);
    }

    public class ServicioPreguntasForos : IServicioPreguntasForos
    {
        private readonly IRepositorioPreguntasForo repositorioPreguntas;
        private readonly IServicioPacientes servicioPacientes;

        public ServicioPreguntasForos(IRepositorioPreguntasForo repositorioPreguntas, IServicioPacientes servicioPacientes)
        {
            this.repositorioPreguntas = repositorioPreguntas;
            this.servicioPacientes = servicioPacientes;
        }

        public async Task<int> CrearPreguntaForo(PreguntaForo preguntaForo)
        {
            return await repositorioPreguntas.CrearPreguntaForo(preguntaForo);
        }

        public async Task<PreguntaForo> ObtenerPreguntaPorId(int id)
        {
            return await repositorioPreguntas.BuscarPreguntaPorId(id);
        }

        public async Task<IEnumerable<PreguntaForo>> ListarPreguntasForo()
        {
            var preguntas = await repositorioPreguntas.ListarPreguntasForo();

            foreach(var preg in preguntas)
            {
                var paciente = await servicioPacientes.ObtenerPacientePorId(preg.PacienteId);
                preg.Paciente = paciente;
            }

            return preguntas;
        }

        public async Task<IEnumerable<PreguntaForo>> ListarCincoUltimasPreguntas()
        {
            return await repositorioPreguntas.ListarCincoUltimasPreguntas();
        }

        public async Task<bool> ActualizarEstadoPregunta(int id)
        {
            return await repositorioPreguntas.ActualizarEstadoPregunta(id);
        }
    }
}
