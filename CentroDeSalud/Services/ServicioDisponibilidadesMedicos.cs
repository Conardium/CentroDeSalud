using CentroDeSalud.Models;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioDisponibilidadesMedicos
    {
        Task<bool> CrearDisponibilidad(DisponibilidadMedico modelo);
        Task<DisponibilidadMedico> ObtenerDisponibilidad(Guid medicoId, int numeroDia);
        Task<IEnumerable<DisponibilidadMedico>> ObtenerHorarioMedico(Guid medicoId);
    }

    public class ServicioDisponibilidadesMedicos : IServicioDisponibilidadesMedicos
    {
        private readonly IRepositorioDisponibilidadesMedicos repositorioDisponibilidades;

        public ServicioDisponibilidadesMedicos(IRepositorioDisponibilidadesMedicos repositorioDisponibilidades)
        {
            this.repositorioDisponibilidades = repositorioDisponibilidades;
        }

        public async Task<bool> CrearDisponibilidad(DisponibilidadMedico modelo)
        {
            return await repositorioDisponibilidades.CrearDisponibilidad(modelo);
        }

        public async Task<DisponibilidadMedico> ObtenerDisponibilidad(Guid medicoId, int numeroDia)
        {
            return await repositorioDisponibilidades.DisponibilidadPorMedicoIdyDiaSemana(medicoId, numeroDia);
        }

        public async Task<IEnumerable<DisponibilidadMedico>> ObtenerHorarioMedico(Guid medicoId)
        {
            return await repositorioDisponibilidades.ObtenerHorarioMedico(medicoId);
        }
    }
}
