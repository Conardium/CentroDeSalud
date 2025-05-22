using CentroDeSalud.Enumerations;
using CentroDeSalud.Infrastructure.Utilidades;
using CentroDeSalud.Models;
using CentroDeSalud.Models.ViewModels;
using CentroDeSalud.Repositories;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Net;

namespace CentroDeSalud.Services
{
    public interface IServicioCitas
    {
        Task ActualizarSincronizarCita(int id);
        Task<Cita> BuscarCitaPorFechaHora(DateTime fecha, TimeSpan hora);
        Task<Cita> BuscarCitaPorId(int id);
        Task<bool> CancelarCita(int id);
        Task<int> CrearCita(Cita cita);
        Task<bool> EliminarCita(int id);
        Task<IEnumerable<Cita>> ListarCitasPorUsuario(Guid id, string rol);
        Task<ResultadoOperacion<List<TimeSpan>>> ListarHorasDisponibles(Guid medicoId, DateTime fecha);
        Task<IEnumerable<SelectPacienteViewModel>> ListarPacientesDelMedico(Guid medicoId);
        Task<IEnumerable<Cita>> ObtenerCitasPendientesPorIdUsuario(Guid usuarioId, string rol);
        Task<ResultadoOperacion<bool>> SincronizarCita(Guid usuarioId, int idCita);
    }

    public class ServicioCitas : IServicioCitas
    {
        private readonly IRepositorioCitas repositorioCitas;
        private readonly IServicioDisponibilidadesMedicos servicioDisponibilidades;
        private readonly IServicioUsuariosLoginsExternos servicioUsuariosLoginsExternos;

        public ServicioCitas(IRepositorioCitas repositorioCitas, IServicioDisponibilidadesMedicos servicioDisponibilidades, 
            IServicioUsuariosLoginsExternos servicioUsuariosLoginsExternos)
        {
            this.repositorioCitas = repositorioCitas;
            this.servicioDisponibilidades = servicioDisponibilidades;
            this.servicioUsuariosLoginsExternos = servicioUsuariosLoginsExternos;
        }

        public async Task<int> CrearCita(Cita cita)
        {
            return await repositorioCitas.CrearCita(cita);
        }

        public async Task<bool> EliminarCita(int id)
        {
            return await repositorioCitas.EliminarCita(id);
        }
        public async Task<IEnumerable<Cita>> ListarCitasPorUsuario(Guid id, string rol)
        {
            return await repositorioCitas.ListarCitasPorUsuario(id, rol);
        }

        public async Task<Cita> BuscarCitaPorId(int id)
        {
            return await repositorioCitas.BuscarCitaPorId(id);
        }

        public async Task<Cita> BuscarCitaPorFechaHora(DateTime fecha, TimeSpan hora)
        {
            return await repositorioCitas.BuscarPorFechaHora(fecha, hora);
        }

        public async Task<IEnumerable<Cita>> ObtenerCitasPendientesPorIdUsuario(Guid usuarioId, string rol)
        {
            return await repositorioCitas.ObtenerCitasPendientesPorIdUsuario(usuarioId, rol);
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
                return ResultadoOperacion<List<TimeSpan>>.Error("<strong>El médico seleccionado no tiene consulta para esta fecha</strong>. Recuerde que puede comprobar la disponibilidad de un médico accediendo al perfil público de este dentro de la web.");

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

        public async Task<ResultadoOperacion<bool>> SincronizarCita(Guid usuarioId, int idCita)
        {
            //Comprobamos primero que el usuario que ha solicitado vincular la cita sea el propio usuario de la cita
            var cita = await repositorioCitas.BuscarCitaPorId(idCita);

            if (cita is null)
                return ResultadoOperacion<bool>.Error("La cita que ha intentado sincronizar no existe.");

            if(cita.PacienteId != usuarioId)
                return ResultadoOperacion<bool>.Error("Ha habido un error al sincronizar la cita.");

            //Comprobamos la caducidad del accessToken
            await servicioUsuariosLoginsExternos.ComprobarCaducidadToken(usuarioId);

            //Si la cita existe en la BD y el usuario es el correcto, procedemos a crear el evento del calendario
            var listaLogins = await servicioUsuariosLoginsExternos.ListadoLoginsExternos(usuarioId);
            var loginGoogle = listaLogins.FirstOrDefault(l => l.LoginProvider == "Google");

            if(loginGoogle is null)
                return ResultadoOperacion<bool>.Error("Vaya, parece que su cuenta no está vinculada con Google.");

            //Implementacion del servicio de Google Calendar
            //Obtenemos la credencial
            var credencial = GoogleCredential.FromAccessToken(loginGoogle.AccessToken);
            
            //Inicializamos el servicio de Calendar
            var servicioCalendar = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credencial,
                ApplicationName = "Cura Vitae"
            });

            //Creamos y configuramos el evento
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");

            var fechaInicioSinZona = cita.Fecha.Date + cita.Hora;
            var offsetMadrid = timeZone.GetUtcOffset(fechaInicioSinZona);

            var fechaInicio = new DateTimeOffset(fechaInicioSinZona, offsetMadrid);
            var fechaFin = fechaInicio.AddMinutes(30);
            var evento = new Event
            {
                Summary = "Consulta médica (Cura Vitae)",
                Location = "",
                Description = cita.Motivo,
                Start = new EventDateTime
                {
                    DateTimeDateTimeOffset = fechaInicio,
                    TimeZone = "Europe/Madrid"
                },
                End = new EventDateTime
                {
                    DateTimeDateTimeOffset = fechaFin,
                    TimeZone = "Europe/Madrid"
                }
            };

            //Enviamos el evento al calendario
            try
            {
                await servicioCalendar.Events.Insert(evento, "primary").ExecuteAsync();
                await ActualizarSincronizarCita(idCita);

            }
            catch (GoogleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.Forbidden)
            {
                throw new HttpRequestException("No se tienen los permisos para acceder al calendario de Google", ex, HttpStatusCode.Forbidden);
            }

            return ResultadoOperacion<bool>.Exito(true);
        }

        public async Task<bool> CancelarCita(int id)
        {
            return await repositorioCitas.CancelarCita(id);
        }

        public async Task ActualizarSincronizarCita(int id)
        {
            await repositorioCitas.SincronizarCita(id);
        }

        public async Task<IEnumerable<SelectPacienteViewModel>> ListarPacientesDelMedico(Guid medicoId)
        {
            return await repositorioCitas.ListarPacientesDelMedico(medicoId);
        }
    }
}
