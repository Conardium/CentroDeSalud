using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Enumerations
{
    public enum Sexo
    {
        Hombre = 0,
        Mujer = 1,
        Otro = 2
    }

    public enum GrupoSanguineo
    {
        [Display(Name = "A+")]
        Apositivo = 0,

        [Display(Name = "A-")]
        Anegativo = 1,

        [Display(Name = "B+")]
        Bpositivo = 2,

        [Display(Name = "B-")]
        Bnegativo = 3,

        [Display(Name = "AB+")]
        ABpositivo = 4,

        [Display(Name = "AB-")]
        ABnegativo = 5,

        [Display(Name = "O+")]
        Opositivo = 6,

        [Display(Name = "O-")]
        Onegativo = 7
    }
}
