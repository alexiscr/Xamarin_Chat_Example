using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheChat.Core.EventHandlers;
using TheChat.Core.Models;
using TheChatMessages;

namespace TheChat.Core.Services
{
    // Métodoa a implementar
    public interface IChatService
    {
        bool IsConnected { get; } // Propiedad que determina si esta conectado o no
        string ConnectionToken { get;  set; } // Propiedad para el token de conexión
        Task InitAsync(string userId); // Tarea que iniciara la conexión a SignalR
        Task DisconnectAsyn(); // Tarea que desconectara del servicio de SignalR
        Task SendMessageAsync(ChatMessage message); // Tarea para el envío de mensajes
        Task JoinChannelAsync(UserConnectedMessage message); // Tarea para el ingreso a un grupo
        Task<List<Room>> GetRooms(); // Tarea para obtener la lista de salas disponibles
        Task LeaveChannelAsync(UserConnectedMessage message); // Tarea para la salir de  una sala
        Task<List<User>> GetUsersGroup(string group); // Tarea para obtener lista de usuarios pertenecientes a un grupo
        Task<User> GetUser(string userId); // Tarea para obetener un usuario en específico 

        event EventHandler<MessageEventArgs> OnReceivedMessage;
    }
}
