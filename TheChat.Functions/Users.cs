using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using TheChat.Functions.Models;
using System.Collections.Generic;

namespace TheChat.Functions
{
    /// <summary>
    /// Función para obtener lista de usuarios 
    /// </summary>
    public static class Users
    {
        // Endpoint de la función
        [FunctionName("Users")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Users/{room}")] HttpRequest req,
            // Tabla a manipular
            [Table("Users", Connection ="StorageConnection")] CloudTable usersTable,
            // Nombre de la sala
            string room,
            ILogger log)
        {
            // Query para obtener usuarios en la sala
            TableQuery<UserEntity> rangeQuery =
                new TableQuery<UserEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", 
                QueryComparisons.Equal, room));      

            // Usuarios obtenidos
            var users = new List<UserEntity>();

            foreach (var entity in await usersTable.ExecuteQuerySegmentedAsync(rangeQuery, null))
            {
                users.Add(entity);
            }

            // Se devuelven los resultados obtenidos
            return new OkObjectResult(users);
        }
    }
}
