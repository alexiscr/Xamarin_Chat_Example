using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace TheChat.Functions
{
    /// <summary>
    /// Función para la negociación de la conexión
    /// </summary>
    public static class NegotiateFunction
    {
        // Endpoint a utilizar
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "negotiate/{userId}")] HttpRequest req,
            // se genera el hubname a utilizar y obtiene la información de conexión
            [SignalRConnectionInfo(HubName = "chat", UserId = "{userId}")] SignalRConnectionInfo connectionInfo,
            ILogger log)
        {

            // Se retorna la información de conexión
            return connectionInfo;
        }
    }
}
