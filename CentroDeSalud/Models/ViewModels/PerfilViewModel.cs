using CentroDeSalud.Enumerations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CentroDeSalud.Models.ViewModels
{
    //Modelo usado para la visualización de los datos en el perfil del usuario
    public class PerfilViewModel : Usuario
    {
        //Datos de ambos
        public string Dni { get; set; }
        public Sexo Sexo { get; set; }

        //Datos del Paciente
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }
        public GrupoSanguineo GrupoSanguineo { get; set; }
        public string Direccion { get; set; }

        //Datos del Médico
        public Especialidad Especialidad { get; set; }
        public ICollection<DisponibilidadMedico> DisponibilidadesMedico { get; set; }
    }
}
