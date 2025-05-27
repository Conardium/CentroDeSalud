using CentroDeSalud.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace CentroDeSalud.Repositories
{
    public interface IRepositorioPublicaciones
    {
        Task<int> CrearPublicacion(Publicacion publicacion);
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

        public async Task<IEnumerable<Publicacion>> ListarPublicaciones()
        {
            using var conexion = new SqlConnection(_connectionString);
            try
            {
                return await conexion.QueryAsync<Publicacion>(@"Select Id, Titulo, Resumen, 
                                        FechaPublicacion, EstadoPublicacion from Publicaciones");
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
    }
}
