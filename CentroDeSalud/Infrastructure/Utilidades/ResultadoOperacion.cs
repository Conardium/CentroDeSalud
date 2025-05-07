namespace CentroDeSalud.Infrastructure.Utilidades
{
    //Clase genérica que sirve para servicios que devuelven datos y a su vez pueden devolver errores
    //en caso de que no se cumplan las condiciones
    public class ResultadoOperacion<T>
    {
        public bool TieneError => !string.IsNullOrEmpty(MensajeError);
        public string MensajeError { get; set; }
        public T Datos { get; set; }

        public static ResultadoOperacion<T> Exito(T datos) => new ResultadoOperacion<T> { Datos = datos };
        public static ResultadoOperacion<T> Error(string mensaje) => new ResultadoOperacion<T> { MensajeError = mensaje };
    }
}
