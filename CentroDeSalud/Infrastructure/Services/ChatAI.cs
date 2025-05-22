using GenerativeAI;

namespace CentroDeSalud.Infrastructure.Services
{
    public interface IChatAI
    {
        Task<string> EnviarMensajeAI(string mensaje);
    }

    public class ChatAI : IChatAI
    {
        private readonly GenerativeModel generativeModel;
        private readonly ILogger<ChatAI> logger;

        public ChatAI(IConfiguration configuration, ILogger<ChatAI> logger)
        {
            this.logger = logger;
            var apiKey = configuration["GoogleAIApiKey"];

            if(string.IsNullOrEmpty(apiKey) )
            {
                logger.LogError("La clave API de Google AI no se ha encontrado");
                throw new InvalidOperationException("La clave de la AI API no se ha encontrado");
            }

            generativeModel = new GenerativeModel(apiKey: apiKey, model: "gemini-1.5-flash");
        }

        public async Task<string> EnviarMensajeAI(string mensaje)
        {
            if (string.IsNullOrEmpty(mensaje))
            {
                return "No hay ningún comentario que analizar.";
            }

            try
            {
                var prompt = $@"Actúa como un Asistente Virtual de Medicina y responde al siguiente comentario de un usuario.

                    Comentario a evaluar: ""{mensaje}"" 

                    Responde de forma estrictamente relacionada con el comentario, utilizando un lenguaje formal, claro, conciso y breve.

                    - Si el comentario es una duda o pregunta, respóndela directamente pero detallando un poco la respuesta.
                    - Si la pregunta requiere explicaciones largas, diagnósticos o información detallada, recomienda pedir una cita con su médico.

                    Evita repetir el comentario original y no salgas del rol de asistente médico.";

                var mensajeIA = await generativeModel.GenerateContentAsync(prompt);

                var resultado = mensajeIA.Candidates[0].Content.Parts[0].Text;

                return resultado;
            }
            catch (Exception ex)
            {
                logger.LogError("La clave API de Google AI no se ha encontrado");
                return "He tenido el siguiente problema para procesar su mensaje: " + ex.Message;
            }
        }
    }
}
