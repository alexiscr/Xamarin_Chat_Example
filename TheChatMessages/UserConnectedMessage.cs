using System;
using System.Collections.Generic;
using System.Text;

namespace TheChatMessages
{
    public class UserConnectedMessage : ChatMessage
    {       
        public  string Token { get; set; }  // Propiedad que almacenara el Token para unirse al grupo
        
        public bool IsEntering { get; set; } // Propiedad que verifica si entra o sale de  un grupo

        public UserConnectedMessage() {} // Constructor Vacio

        public UserConnectedMessage(string userName, string groupName) : base(userName)
        {
            GroupName = groupName; // Se obtiene el grupo el cual se gestionara
        }
    }
}
