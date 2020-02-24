using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TheChat.Functions.Helpers;

namespace TheChat.Functions
{
    /// <summary>
    /// Función que se disparara a treves de un timer trigger para eliminar las fotografias almacenadas
    /// </summary>
    public static class ClearPhotos
    {
        // Endpoint que limpiara las fotografias cada 60 min (1h)
        [FunctionName("ClearPhotos")]
        public async static void Run([TimerTrigger("0 */60 * * * *")]TimerInfo myTimer, ILogger log)
        {
            await StorageHelper.Clear();
        }
    }
}
