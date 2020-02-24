using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TheChat.Functions.Models;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using TheChatMessages;

namespace TheChat.Functions
{
    /// <summary>
    /// Función para agregar usuarios a un grupo
    /// </summary>
    public static class AddToGroup
    {
        // Nombre del endpoint
        [FunctionName("AddToGroup")]
        // Establecemos conexión a la tabla a utilizar en el Azure Storage
        [return: Table("Users", Connection = "StorageConnection")]
        public static async Task<UserEntity> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            // Se selecciona el Hub a utilizar
            [SignalR(HubName = "chat")] IAsyncCollector<SignalRGroupAction> signalRGroupActions,
            ILogger log)
        {
            // se lee el mensaje a traves del Body Request
            var message = new JsonSerializer()
                .Deserialize<UserConnectedMessage>(
                    new JsonTextReader(new StreamReader(req.Body)));
            
            // Se ejecuta el grupo de acciones
            await signalRGroupActions.AddAsync(new SignalRGroupAction
            {
                ConnectionId = message.Token,
                UserId = message.Sender,
                GroupName = message.GroupName,
                Action = GroupAction.Add
            });

            // Se genera un color random al usuario
            Random r = new Random();
            var red = r.Next(0, 255).ToString("X2");
            var green = r.Next(0, 255).ToString("X2");
            var blue = r.Next(0, 255).ToString("X2");

            // Se genera la nueva identidad a almacenea en la tabla
            var item = new UserEntity
            {
                UserId = message.Sender,
                Room = message.GroupName,
                Color = $"#{red}{green}{blue}",     
                Avatar=$"image_{r.Next(1, 51)}.png",
                PartitionKey = message.GroupName,
                RowKey = message.Sender
            };
            // Se retorna el item 
            return item;
        }
    }
}
