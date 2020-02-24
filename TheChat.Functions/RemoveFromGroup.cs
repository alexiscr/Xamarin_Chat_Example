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
using TheChatMessages;
using Microsoft.WindowsAzure.Storage.Table;
using TheChat.Functions.Models;

namespace TheChat.Functions
{
    /// <summary>
    /// Función para remover un usuario del grupo
    /// </summary>
    public static class RemoveFromGroup
    {

        // Endpoint a utilizar
        [FunctionName("RemoveFromGroup")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            // Se estable el hub a procesar
            [SignalR(HubName = "chat")] IAsyncCollector<SignalRGroupAction> signalRGroupActions,
            // Tabla a manipular
            [Table("Users",Connection = "StorageConnection")] CloudTable usersTable,
            ILogger log)
        {
            // Se deserializa el mensaje
            var message = new JsonSerializer()
                 .Deserialize<UserConnectedMessage>(
                     new JsonTextReader(new StreamReader(req.Body)));

            // Se ejecuta el grupo de acciones
            await signalRGroupActions.AddAsync(new SignalRGroupAction
            {
                ConnectionId = message.Token,
                UserId = message.Sender,
                GroupName = message.GroupName,
                Action = GroupAction.Remove
            });

            // Eliminar al usuario e la tabla
            TableQuery<UserEntity> rangeQuery = new TableQuery<UserEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, message.Sender));

            foreach (var entity in await usersTable
                .ExecuteQuerySegmentedAsync(rangeQuery, null))
            {
                TableOperation deleteOperation = TableOperation.Delete(entity);
                TableResult result = await usersTable.ExecuteAsync(deleteOperation);
            }
        }
    }
}
