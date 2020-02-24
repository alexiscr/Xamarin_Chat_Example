using System;
using System.Collections.Generic;
using System.Text;

namespace TheChatMessages
{
    // Clase Padre de mensages
    public class ChatMessage
    {
        public string Id{ get; set; }
        public Type TypeInfo { get; set; }
        public DateTime Timestamp { get; set; }
        public string Sender { get; set; } // Propiedad que representa el nombre del usuario
        public string GroupName { get; set; } // Propiedad para almacenar el grupo seleccionado por el usuario     
        public string Recipient { get; set; } // Propiead para determinar el destinatario del mensaje privado
        public bool IsPrivate { get; set; } // Propiedad que determina si el mensaje es privado o no
        public string Color { get; set; }
        public string Avatar { get; set; }
        public ChatMessage() {} // Constructor Vacio

        public ChatMessage(String sender) // Constructor de la clase base
        {
            Id = Guid.NewGuid().ToString(); // Genera un Globally Unique Identifier
            TypeInfo = GetType(); // Obtenemos el tipo de la instancia actual del objeto
            Sender = sender;
            Timestamp = DateTime.Now; // Se captura la fecha actual del sistema
        }
    }
}
