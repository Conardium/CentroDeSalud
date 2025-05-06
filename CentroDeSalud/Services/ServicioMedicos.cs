using CentroDeSalud.Models;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioMedicos
    {
        Task<IEnumerable<Medico>> ListarMedicos();
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
    }
}
