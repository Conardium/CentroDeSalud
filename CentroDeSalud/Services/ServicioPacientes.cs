using CentroDeSalud.Models;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioPacientes
    {
        Task<Guid> CrearPacienteAsync(Paciente paciente);
    }

    public class ServicioPacientes : IServicioPacientes
    {
        public readonly IRepositorioPacientes _repositorioPacientes;

        public ServicioPacientes(IRepositorioPacientes repositorioPacientes)
        {
            _repositorioPacientes = repositorioPacientes;
        }

        public async Task<Guid> CrearPacienteAsync(Paciente paciente)
        {
            return await _repositorioPacientes.CrearPaciente(paciente);
        }
    }
}
