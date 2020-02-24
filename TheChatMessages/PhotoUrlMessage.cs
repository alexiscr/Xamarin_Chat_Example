using System;
using System.Collections.Generic;
using System.Text;

namespace TheChatMessages
{

    // Clase que obtiene el Url de uan imagen enviada
    public class PhotoUrlMessage : ChatMessage
    {

        public string Url { get; set; } // Propiedad que almacenara la url de una imagen posteada por otro usuario
        
        public PhotoUrlMessage() { } // Constructor Vacio
        public PhotoUrlMessage(string sender) : base(sender) {} // Constructor que llama al constructor de la clase base
    }
}
