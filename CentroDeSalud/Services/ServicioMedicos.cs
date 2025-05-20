using CentroDeSalud.Models;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioMedicos
    {
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

        public async Task<IEnumerable<Medico>> ListarMedicos()
        {
            return await repositorioMedicos.ListadoMedicos();
        }

        public async Task<Medico> ObtenerMedicoPorId(Guid id)
        {
            return await repositorioMedicos.ObtenerMedicoPorId(id);
        }
    }
}
