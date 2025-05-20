using CentroDeSalud.Data;
using CentroDeSalud.Models;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioInformes
    {
        Task<int> CrearInforme(Informe informe);
        Task<bool> EditarInforme(Informe informe);
        Task<IEnumerable<Informe>> ListarInformesPorUsuario(Guid usuarioId, string rol);
        Task<Informe> ObtenerInformePorId(int idInforme);
    }

    public class ServicioInformes : IServicioInformes
    {
        private readonly IRepositorioInformes repositorioInformes;

        public ServicioInformes(IRepositorioInformes repositorioInformes)
        {
            this.repositorioInformes = repositorioInformes;
        }

        public async Task<int> CrearInforme(Informe informe)
        {
            return await repositorioInformes.CrearInforme(informe);
        }

        public async Task<bool> EditarInforme(Informe informe)
        {
            return await repositorioInformes.EditarInforme(informe);
        }

        public async Task<IEnumerable<Informe>> ListarInformesPorUsuario(Guid usuarioId, string rol)
        {
            if (rol == Constantes.RolPaciente)
            {
                return await repositorioInformes.ListarInformesDelPaciente(usuarioId);
            }
            else if (rol == Constantes.RolMedico)
            {
                return await repositorioInformes.ListarInformesDelMedico(usuarioId);
            }
            else
            {
                return await repositorioInformes.ListarInformes();
            }
        }

        public async Task<Informe> ObtenerInformePorId(int idInforme)
        {
            return await repositorioInformes.ObtenerInformePorId(idInforme);
        }
    }
}
