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
}
