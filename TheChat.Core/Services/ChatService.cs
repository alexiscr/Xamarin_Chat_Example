using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheChat.Core.EventHandlers;
using TheChat.Core.Models;
using TheChatMessages;

namespace TheChat.Core.Services
{
    // Clase para el manejo de servicios de Chat
    public class ChatService : IChatService
    {
        public bool IsConnected { get; set; } // Propiedad que maneja el estado de conexión
        public string ConnectionToken { get; set; } // Propiedad para el manejo del token de la conexión

        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1); // Variable que limita el uso de hilos

        private HttpClient httpClient; // Variable que estable el cliente HTTP sobre los endpoints
        
        HubConnection hub; // Variable que permite la  la inicializacion al hub "chat"
        
        public event EventHandler<MessageEventArgs> OnReceivedMessage; // Evento qu egestiona los mensages recividos

        /// <summary>
        /// Método para iniciar la conexión al servicio de SignalR
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task InitAsync(string userId)
        {
            await semaphoreSlim.WaitAsync(); // Se establece el limite de hilos
            
            // Se instancia el cliente HTTP en caso que ya exista el objeto se procede
            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }

            // Se ejecuta el endpoint "GET" encargado de la negociación al servicio de SignalR
            var result = await httpClient.GetStringAsync($"{ Config.NegotiateEndpoint}/{userId}");

            // Se deserializa la información obtenida
            var info = JsonConvert.DeserializeObject<ConnectionInfo>(result);

            // Se instancia un nuevo objeto para crear el Hub de conexión
            var connectionBuilder = new HubConnectionBuilder();

            // Configuración HUB y del transporte HTTPa conectar en la url especifica
            connectionBuilder.WithUrl(info.Url, (obj) => {
                obj.AccessTokenProvider = () => Task.Run(()=> info.AccessToken);
            });

            // Se procede a construir el HUB
            hub = connectionBuilder.Build();
           
            // Inicialiaza la conexión con el hub
            await hub.StartAsync();

            // Se obtiene el token de conexión del hub
            ConnectionToken = hub.ConnectionId;

            // Se estable que la conexión ha sido iniciada
            IsConnected = true;

           
            hub.On<object>("ReceiveMessage",(message)=>{

                // Se obtiene el mensaje
                var json = message.ToString();

                // Obtengo el mensaje general
                var obj = JsonConvert.DeserializeObject<ChatMessage>(json);

                // Convertimos al tipo de mensage enviado 
                var msg = (ChatMessage)JsonConvert.DeserializeObject(json, obj.TypeInfo);

                // Lamamos el evento que reportara el nuevo mensaje
                OnReceivedMessage?.Invoke(this, new MessageEventArgs(msg));

            });

            // Se libera el limite de hilos 
            semaphoreSlim.Release();
        }
        
        /// <summary>
        /// Método para la desconección al serivicio de SignalR
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectAsyn()
        {
            // Se verifica que en realidad se este conectado
            if (!IsConnected)
                return;

            try
            {
                // Se libera el hub
                await hub.DisposeAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            // Se pasa a falso la propiedad que verifica la conexión
            IsConnected = false;
        }

        /// <summary>
        /// Método para el envio de mensajes
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(ChatMessage message)
        {
            // Primero se verifica si estamos conectado , 
            // en caso contrario se lanza una excepción de operación invalida
            if (!IsConnected)
            {
                throw new InvalidOperationException("Not Connected");
            }

            // Se serialia el objeto mensaje a JSON
            var json = JsonConvert.SerializeObject(message);

            // Se da formato al mensaje a enviar
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Se envía a traves de un "POST"
            await httpClient.PostAsync(Config.MessagesEndpoint, content);
        }

        /// <summary>
        /// Método que permite la unión a la sala de chat correspondiente
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task JoinChannelAsync(UserConnectedMessage message)
        {
            // se verifica que la conexión este establecida
            if (!IsConnected)            
                return;
            
            // Se estable el token de conexión
            message.Token = ConnectionToken;
            
            // Se estable que el usuario a entrado a un sala
            message.IsEntering = true;

            // Se serializa el objeto a enviar
            var json = JsonConvert.SerializeObject(message);

            // Se convierte a JSON el contenido
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Se ejecuta el envio a traves del endpoint correspondiente "POST"
            await httpClient.PostAsync(Config.AddToGroupEndPoint, content);

            // Se envia el mensaje para informar que se han conectado
            await SendMessageAsync(message);
        }

        /// <summary>
        /// Método para obtener lista de salas disponibles
        /// </summary>
        /// <returns></returns>
        public async Task<List<Room>> GetRooms()
        {
            // Creación de la lista de salas
            var rooms = new List<Room>
            {
                new Room { Name = "C#", Image = "csharp.png"},
                new Room { Name = "Xamarin", Image = "xamarin.png"},
                new Room { Name = ".Net", Image = "dotnet.png"},
                new Room { Name = "ASPE.NET Core", Image = "aspcore.png"},
                new Room { Name = "Xamarin", Image = "xamforms.png"},
                new Room { Name = "Visual Studio", Image = "visualstudio.png"}
            };

            // Se obtiene el número de usuarios por sala
            foreach (var room in rooms)
            {
                room.UsersNumber = await GetRoomCount(room.Name);
            }

            // Se retorna la lista de salas
            return rooms;
        }

        /// <summary>
        /// Método para salir de una sala de chat 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task LeaveChannelAsync(UserConnectedMessage message)
        {
            // Verificamos que aun sea una conexión activa
            if (!IsConnected)            
                return;

            // Se establece el Token de conexión correspondiente
            message.Token = ConnectionToken;

            // Se serializa el objeto mensaje 
            var json = JsonConvert.SerializeObject(message);
            
            // se genera el contenido JSON
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Se ejecuta POST del contenido generado
            await httpClient.PostAsync(Config.LeaveGroupEndpoint, content);

            // Se envia el mensaje que dara a conocer la desconexión
            await SendMessageAsync(message);
        }

        /// <summary>
        /// Método para obtener la lista de usuarios de una sala de chat
        /// </summary>
        /// <param name="group">nombre de la sala de chat</param>
        /// <returns></returns>
        public async Task<List<User>> GetUsersGroup(string group)
        {
            // Url correspondiente al endpoint de la función
            var url = $"{Config.RoomsEndpoint}/{group}";

            // Se obtiene el resultado a traves de un "GET"
            var result = await httpClient.GetStringAsync(url);

            // Se deserializa el objeto para obtener la lista de los usuarios
            var users = JsonConvert.DeserializeObject<List<User>>(result);

            // Se retorna la lista de usuarios
            return users;
        }

        /// <summary>
        /// Método para obtener el número de usuarios en la sala seleccionada
        /// </summary>
        /// <param name="group">Nombre de la sala</param>
        /// <returns></returns>
        private async Task<int> GetRoomCount(string group) {

            // Se llama a la función que obtiene la lista de usuarios por grupo
            var users = await GetUsersGroup(group);

            // Se retorna el número de usuarios a traves del método count
            return users.Count;
        }

        /// <summary>
        /// Método para obtener un usuario en especifico
        /// </summary>
        /// <param name="userId">Nombre del usuario a obtener el objeto</param>
        /// <returns></returns>
        public async Task<User> GetUser(string userId)
        {
            // Url correspondiente al endpoint para obtener el usuario
            var url = $"{Config.UserEndpoint}/{userId}";

            // Se obtiene el resultado a traves de "GET"
            var result = await httpClient.GetStringAsync(url);

            // Se obtiene la lista de usuarios
            var users = JsonConvert.DeserializeObject<List<User>>(result);

            // Se selecciona el primer registro
            return users.FirstOrDefault();
        }
    }
}
