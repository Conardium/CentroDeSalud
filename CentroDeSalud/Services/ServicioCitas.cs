using CentroDeSalud.Enumerations;
using CentroDeSalud.Infrastructure.Utilidades;
using CentroDeSalud.Models;
using CentroDeSalud.Repositories;
using System.Globalization;

namespace CentroDeSalud.Services
{
    public interface IServicioCitas
    {
        Task<Cita> BuscarCitaPorFechaHora(DateTime fecha, TimeSpan hora);
        Task<int> CrearCita(Cita cita);
        Task<ResultadoOperacion<List<TimeSpan>>> ListarHorasDisponibles(Guid medicoId, DateTime fecha);
    }

    public class ServicioCitas : IServicioCitas
    {
        private readonly IRepositorioCitas repositorioCitas;
        private readonly IServicioDisponibilidadesMedicos servicioDisponibilidades;

        public ServicioCitas(IRepositorioCitas repositorioCitas, IServicioDisponibilidadesMedicos servicioDisponibilidades)
        {
            this.repositorioCitas = repositorioCitas;
            this.servicioDisponibilidades = servicioDisponibilidades;
        }

        public async Task<int> CrearCita(Cita cita)
        {
            return await repositorioCitas.CrearCita(cita);
        }

        public async Task<Cita> BuscarCitaPorFechaHora(DateTime fecha, TimeSpan hora)
        {
            return await repositorioCitas.BuscarPorFechaHora(fecha, hora);
        }

        public async Task<ResultadoOperacion<List<TimeSpan>>> ListarHorasDisponibles(Guid medicoId, DateTime fecha)
        {
            //Comprobamos que la fecha no sea anterior a la de hoy
            if(fecha.Date < DateTime.Now.Date)
                return ResultadoOperacion<List<TimeSpan>>.Error("No se puede pedir cita para una fecha pasada");

            //Recogemos la disponibilidad del medico segun el dia de la semana
            var diaSemana = fecha.ToString("dddd", new CultureInfo("es-ES"));
            var diaSemanaCapitalizado = char.ToUpper(diaSemana[0]) + diaSemana.Substring(1);

            int numeroDia = 0;
            if(Enum.TryParse(diaSemanaCapitalizado, out DiaSemana dia))
                numeroDia = (int)dia;

            if (numeroDia == 0)
                return ResultadoOperacion<List<TimeSpan>>.Error("El día seleccionado no es válido.");

            var disponibilidadMedico = await servicioDisponibilidades.ObtenerDisponibilidad(medicoId, numeroDia);

            if (disponibilidadMedico is null)
                return ResultadoOperacion<List<TimeSpan>>.Error("<strong>El médico no trabaja para el día seleccionado</strong>. Recuerde que puede comprobar la disponibilidad de un médico accediendo al perfil público de este dentro de la web.");

            //Creamos las posibles franjas horarias de 30min
            TimeSpan horaInicio = disponibilidadMedico.HoraInicio;
            TimeSpan horaFin = disponibilidadMedico.HoraFin;
            var franjasHorarias = new List<TimeSpan>();

            //Si la cita se pide para HOY comprobamos si la hora en la que se pide la cita es mayor
            //a la inicial y/o mayor a la final
            if(DateTime.Now.Date == fecha.Date)
            {
                var horaActual = DateTime.Now.TimeOfDay;
                //Si la horaActual es mayor a la hora de fin de las citas para esa fecha devolvemos NULL
                if (horaActual > horaFin)
                    return ResultadoOperacion<List<TimeSpan>>.Error("El horario de citas para hoy ya ha finalizado. Le rogamos que pida cita para una fecha posterior.");

                if(horaActual > horaInicio)
                {
                    var nuevaHoraInicio = horaActual.Minutes <= 15
                        ? new TimeSpan(horaActual.Hours + 1, 30, 0) //Ejemplo: 9:10 => 10:30
                        : new TimeSpan(horaActual.Hours + 2, 0, 0); //Ejemplo: 9:30 => 11:00

                    horaInicio = nuevaHoraInicio;
                }
            }

            for (var hora = horaInicio; hora < horaFin; hora = hora.Add(TimeSpan.FromMinutes(30)))
                franjasHorarias.Add(hora);
            

            //Cogemos las citas pendientes que tenga el medico ese dia
            var citasPendientesMedico = await repositorioCitas.ObtenerCitasPendientesMedicoPorFecha(medicoId, fecha);

            //Filtramos las franjas horarias y seleccionamos solo las que no estén pilladas
            var franjasDisponibles = franjasHorarias
                .Where(franja => !citasPendientesMedico.Any(cita => cita.Hora == franja)).ToList();

            if (!franjasDisponibles.Any())
                return ResultadoOperacion<List<TimeSpan>>.Error("Lo sentimos, pero el médico que ha seleccionado no dispone de más citas para esta fecha");
            

            return ResultadoOperacion<List<TimeSpan>>.Exito(franjasDisponibles);
        }
    }
}
