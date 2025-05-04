using CentroDeSalud.Models;
using CentroDeSalud.Repositories;

namespace CentroDeSalud.Services
{
    public interface IServicioPaciente
    {
        Task<Guid> CrearPacienteAsync(Paciente paciente);
    }

    public class ServicioPaciente : IServicioPaciente
    {
        public readonly IRepositorioPacientes _repositorioPacientes;

        public ServicioPaciente(IRepositorioPacientes repositorioPacientes)
        {
            _repositorioPacientes = repositorioPacientes;
        }

        public async Task<Guid> CrearPacienteAsync(Paciente paciente)
        {
            return await _repositorioPacientes.CrearPaciente(paciente);
        }
    }
}
