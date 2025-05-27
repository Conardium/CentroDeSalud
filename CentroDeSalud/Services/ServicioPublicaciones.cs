using CentroDeSalud.Repositories;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using CentroDeSalud.Models;

namespace CentroDeSalud.Services
{
    public interface IServicioPublicaciones
    {
        Task<int> CrearPublicacion(Publicacion publicacion);
        string GenerarSlug(string titulo);
        Task<IEnumerable<Publicacion>> ListarDestacadas();
        Task<IEnumerable<Publicacion>> ListarPublicaciones();
    }

    public class ServicioPublicaciones : IServicioPublicaciones
    {
        private readonly IRepositorioPublicaciones repositorioPublicaciones;

        public ServicioPublicaciones(IRepositorioPublicaciones repositorioPublicaciones)
        {
            this.repositorioPublicaciones = repositorioPublicaciones;
        }

        //Funcion que permite transformar el titulo de la publicacion en una URL amigable
        //Quitando los acentos, convertiendolo todo a minusculas, quitando los espacion y convertiendolos en -, etc.
        public string GenerarSlug(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return string.Empty;

            // Convertir a minúsculas
            string slug = titulo.ToLowerInvariant();

            // Quitar acentos
            slug = RemoveDiacritics(slug);

            // Reemplazar cualquier cosa que no sea letra o número por guiones
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Reemplazar espacios múltiples o guiones por uno solo
            slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-');

            return slug;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task<int> CrearPublicacion(Publicacion publicacion)
        {
            return await repositorioPublicaciones.CrearPublicacion(publicacion);
        }

        public async Task<IEnumerable<Publicacion>> ListarPublicaciones()
        {
            return await repositorioPublicaciones.ListarPublicaciones();
        }

        public async Task<IEnumerable<Publicacion>> ListarDestacadas()
        {
            return await repositorioPublicaciones.ListarDestacadas();
        }
    }
}
