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
using TheChat.Functions.Helpers;

namespace TheChat.Functions
{
    /// <summary>
    /// Función para el envio e messages
    /// </summary>
    public static class Messages
    {
        // Endpoint de mensajes
        [FunctionName("Messages")]
        public static async Task Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            // Establecemos el hub de conexión
            [SignalR(HubName ="chat")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            // Se obtiene el contenido del Request Body
            var serializeObject = new JsonSerializer().Deserialize(new JsonTextReader(new StreamReader(req.Body)));

            // Se deserializa el mensaje obtenido del body
            var message = JsonConvert.DeserializeObject<ChatMessage>(serializeObject.ToString());

            // Verificamos que tipo de mensaje es acorde a la clase y asi poder procesarlo
            if (message.TypeInfo.Name == nameof(UserConnectedMessage))
            {
                // Se deserializa el tipo obtenido
                message = JsonConvert.DeserializeObject<UserConnectedMessage>(serializeObject.ToString());

                // Se agrega la funcion de signalR
                await signalRMessages.AddAsync(new SignalRMessage
                {
                    GroupName = message.GroupName,
                    Target = "ReceiveMessage",
                    Arguments = new[] { message }
                });
            }
            else if (message.TypeInfo.Name == nameof(SimpleTextMessage))
            {
                // Se deserializa el tipo obtenido
                message = JsonConvert.DeserializeObject<SimpleTextMessage>(serializeObject.ToString());

                var signalRMessage = new SignalRMessage
                {
                    // Nombre del metodo que agregara el mensaje al chat
                    Target = "ReceiveMessage",
                    // Es el mensaje a enviar por Signal R
                    Arguments = new[] { message }

                };
                // Unicamente se pueden configurar estas propiedades una a la vez
                if (message.GroupName != null)
                {
                    signalRMessage.GroupName = message.GroupName;
                }
                else if (message.Recipient != null) {
                    signalRMessage.UserId = message.Recipient;
                }

                // Se agrega a la función a signalR
                await signalRMessages.AddAsync(signalRMessage);

            } 
            else if (message.TypeInfo.Name == nameof(PhotoMessage) ){

                // Se deserializa el tipo obtenido
                var photoMessage = JsonConvert.DeserializeObject<PhotoMessage>(serializeObject.ToString());

                // se convierte a bytes la propiedad de Photo
                var bytes = Convert.FromBase64String(photoMessage.Base64Photo);

                var url = await StorageHelper.Upload(bytes, photoMessage.FileEnding);

                // Se establecen las propiedades del mesaje
                message = new PhotoUrlMessage(photoMessage.Sender) { 
                    Id = photoMessage.Id,
                    Timestamp = photoMessage.Timestamp,
                    Url = url,
                    GroupName = photoMessage.GroupName
                };

                // Se agrega a la función de signalR
                await signalRMessages.AddAsync(new SignalRMessage {
                    GroupName = message.GroupName,
                    Target ="ReceiveMessage",
                    Arguments = new[] { message }
                });
                                     
            }                        
        }
    }
}
