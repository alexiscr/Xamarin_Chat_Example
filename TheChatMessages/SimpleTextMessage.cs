using System;
using System.Collections.Generic;
using System.Text;

namespace TheChatMessages
{
    // Clase para mensaje simple
    public class SimpleTextMessage : ChatMessage
    {
        public string Text { get; set; } // Propiedad que almacena el text enviado como mensaje
        
        public SimpleTextMessage() { } // Constructor Vacio        
        public SimpleTextMessage(string sender) : base(sender) { } // Constructor que llama al constructor de la clase base        
    }
}
