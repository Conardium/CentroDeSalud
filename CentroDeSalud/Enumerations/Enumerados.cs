using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Enumerations
{
    public enum Sexo
    {
        [Display(Name = "Seleccione un sexo")]
        NoSeleccionado = 0,
        Hombre = 1,
        Mujer = 2,
        Otro = 3
    }

    public enum Especialidad
    {
        [Display(Name = "Seleccione una especialidad")]
        NoSeleccionado = 0,
        [Display(Name = "Medicina General")]
        MedicinaGeneral = 1,
        Pediatría = 2,
        Ginecología = 3,
        Cardiología = 4,
        Dermatología = 5,
        Psiquiatría = 6
    }

    public enum GrupoSanguineo
    {
        [Display(Name = "Seleccione un grupo sanguíneo")]
        NoSeleccionado = 0,

        [Display(Name = "A+")]
        Apositivo = 1,

        [Display(Name = "A-")]
        Anegativo = 2,

        [Display(Name = "B+")]
        Bpositivo = 3,

        [Display(Name = "B-")]
        Bnegativo = 4,

        [Display(Name = "AB+")]
        ABpositivo = 5,

        [Display(Name = "AB-")]
        ABnegativo = 6,

        [Display(Name = "O+")]
        Opositivo = 7,

        [Display(Name = "O-")]
        Onegativo = 8
    }

    public enum EstadoCita
    {
        Pendiente = 0,
        Finalizada = 1,
        Cancelada = 2,
    }

    public enum DiaSemana
    {
        [Display(Name = "Seleccione un día de la semana")]
        NoSeleccionado = 0,
        Lunes = 1,
        Martes = 2,
        Miércoles = 3,
        Jueves = 4,
        Viernes = 5,
        Sábado = 6,
        Domingo = 7
    }

    public enum EstadoInforme
    {
        Borrador = 0,
        [Display(Name = "En revisión")]
        EnRevision = 1,
        Definitivo = 2
    }
}
