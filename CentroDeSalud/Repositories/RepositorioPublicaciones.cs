using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;


namespace CentroDeSalud.Repositories
{
    public interface IRepositorioPublicaciones
    {
        Task<bool> ActualizarPublicacion(Publicacion publicacion);
        Task<Publicacion> BuscarPublicacionPorId(int id);
        Task<Publicacion> BuscarPublicacionPorSlug(string slug);
        Task<int> CrearPublicacion(Publicacion publicacion);
        Task<bool> EliminarPublicacion(int id);
        Task<IEnumerable<Publicacion>> ListarDestacadas();
        Task<IEnumerable<Publicacion>> ListarPublicaciones();
    }

    public class RepositorioPublicaciones : IRepositorioPublicaciones
    {
        private readonly string _connectionString;

        public RepositorioPublicaciones(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DevelopmentConnection");
        }

        public async Task<int> CrearPublicacion(Publicacion publicacion)
        {
            using var conexion = new SqlConnection(_connectionString);

            try
            {
                var id = await conexion.QuerySingleAsync<int>(@"INSERT INTO Publicaciones (Titulo, Cuerpo, Resumen, FechaPublicacion,
                      Destacada, UsuarioId, Slug, EstadoPublicacion, ImagenURL) 
                        VALUES (@Titulo, @Cuerpo, @Resumen, @FechaPublicacion, @Destacada, @UsuarioId, @Slug, 
                        @EstadoPublicacion, @ImagenURL);
                      select SCOPE_IDENTITY();", publicacion);

                return id;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<bool> EliminarPublicacion(int id)
        {
            using var conexion = new SqlConnection(_connectionString);

            try
            {
                await conexion.ExecuteAsync(@"Delete from Publicaciones where Id = @Id", new {Id = id});
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Publicacion> BuscarPublicacionPorId(int id)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                return await conexion.QueryFirstOrDefaultAsync<Publicacion>(@"Select * from Publicaciones
                                            where Id = @Id", new {Id = id});
            }
            catch
            {
                return null;
            }
        }

        public async Task<Publicacion> BuscarPublicacionPorSlug(string slug)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                return await conexion.QueryFirstOrDefaultAsync<Publicacion>(@"Select * from Publicaciones
                                            where Slug = @Slug", new { Slug = slug });
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<Publicacion>> ListarPublicaciones()
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                return await conexion.QueryAsync<Publicacion>(@"Select Id, Titulo, Resumen, FechaPublicacion, 
                                FechaModificacion, EstadoPublicacion, Slug, Destacada from Publicaciones");
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<Publicacion>> ListarDestacadas()
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                return await conexion.QueryAsync<Publicacion>(@"Select * from Publicaciones where Destacada = 1");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ActualizarPublicacion(Publicacion publicacion)
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                var modImagen = publicacion.ImagenURL.IsNullOrEmpty() ? "" : ", ImagenURL = @ImagenURL";

                await conexion.ExecuteAsync(@"Update Publicaciones Set Titulo = @Titulo, Cuerpo = @Cuerpo,
                        Resumen = @Resumen, FechaModificacion = @FechaModificacion, Slug = @Slug, Destacada = @Destacada,
                        EstadoPublicacion = @EstadoPublicacion" + modImagen + " Where Id = @Id", publicacion);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
