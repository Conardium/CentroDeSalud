using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioMedicos
    {
        Task<bool> ActualizarDatosPerfil(EditarPerfilViewModel modelo);
        Task<Guid> CrearMedico(Medico medico);
        Task<IEnumerable<Medico>> ListarMedicos();
        Task<Medico> ObtenerMedicoPorId(Guid id);
    }

    public class ServicioMedicos : IServicioMedicos
    {
        private readonly IRepositorioMedicos repositorioMedicos;

        public ServicioMedicos(IRepositorioMedicos repositorioMedicos)
        {
            this.repositorioMedicos = repositorioMedicos;
        }

        public async Task<Guid> CrearMedico(Medico medico)
        {
            return await repositorioMedicos.CrearMedico(medico);
        }

        public async Task<IEnumerable<Medico>> ListarMedicos()
        {
            return await repositorioMedicos.ListadoMedicos();
        }

        public async Task<Medico> ObtenerMedicoPorId(Guid id)
        {
            return await repositorioMedicos.ObtenerMedicoPorId(id);
        }

        public async Task<bool> ActualizarDatosPerfil(EditarPerfilViewModel modelo)
        {
            return await repositorioMedicos.ActualizarDatosPerfil(modelo);
        }
    }
}
