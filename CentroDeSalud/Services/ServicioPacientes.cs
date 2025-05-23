using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioPacientes
    {
        Task<bool> ActualizarDatosPerfil(EditarPerfilViewModel modelo);
        Task<Guid> CrearPacienteAsync(Paciente paciente);
        Task<Paciente> ObtenerPacientePorId(Guid id);
    }

    public class ServicioPacientes : IServicioPacientes
    {
        public readonly IRepositorioPacientes repositorioPacientes;

        public ServicioPacientes(IRepositorioPacientes repositorioPacientes)
        {
            this.repositorioPacientes = repositorioPacientes;
        }

        public async Task<Guid> CrearPacienteAsync(Paciente paciente)
        {
            return await repositorioPacientes.CrearPaciente(paciente);
        }

        public async Task<Paciente> ObtenerPacientePorId(Guid id)
        {
            return await repositorioPacientes.ObtenerPacientePorId(id);
        }

        public async Task<bool> ActualizarDatosPerfil(EditarPerfilViewModel modelo)
        {
            return await repositorioPacientes.ActualizarDatosPerfil(modelo);
        }
    }
}
