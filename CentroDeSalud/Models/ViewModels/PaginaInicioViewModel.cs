namespace CentroDeSalud.Models.ViewModels
{
    public class PaginaInicioViewModel
    {
        public ICollection<Publicacion> Destacadas { get; set; }

        public ICollection<Publicacion> Publicaciones { get; set; }
    }
}
