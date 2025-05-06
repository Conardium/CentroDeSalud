using CentroDeSalud.Models;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioDisponibilidadesMedicos
    {
        Task<DisponibilidadMedico> ObtenerDisponibilidad(Guid medicoId, int numeroDia);
    }

    public class ServicioDisponibilidadesMedicos : IServicioDisponibilidadesMedicos
    {
        private readonly IRepositorioDisponibilidadesMedicos repositorioDisponibilidades;

        public ServicioDisponibilidadesMedicos(IRepositorioDisponibilidadesMedicos repositorioDisponibilidades)
        {
            this.repositorioDisponibilidades = repositorioDisponibilidades;
        }

        public async Task<DisponibilidadMedico> ObtenerDisponibilidad(Guid medicoId, int numeroDia)
        {
            return await repositorioDisponibilidades.DisponibilidadPorMedicoIdyDiaSemana(medicoId, numeroDia);
        }
    }
}
