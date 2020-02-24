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
    ///  Funcion para obtener un usuario
    /// </summary>
    public static class User
    {
        // Endpoint de la función
        [FunctionName("User")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/{userId}")] HttpRequest req,
            //Se estable la tabla a manipular
            [Table("Users", Connection = "StorageConnection")] CloudTable usersTable,
            string userId,
            ILogger log)
        {
            // Query para obetener el usuario desde la tabla
            TableQuery<UserEntity> rangeQUery = new TableQuery<UserEntity>()
                .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId));

            var users = new List<UserEntity>();

            foreach (var entity in await usersTable.ExecuteQuerySegmentedAsync(rangeQUery, null))
            {
                users.Add(entity);
            }
            // Se retorna el objeto obtenido
            return new OkObjectResult(users);
        }
    }
}
