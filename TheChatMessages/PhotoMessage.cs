using System;
using System.Collections.Generic;
using System.Text;

namespace TheChatMessages
{

    // Clase para el manejo de mensajes del tipo Photo
    public class PhotoMessage : ChatMessage
    {
        public string Base64Photo { get; set; } // Propiedad que almacenara el string base64 de la imagen enviada
        public string FileEnding { get; set; } // Propiedad que almacenara la extension de la imagen enviada
        public PhotoMessage() {} // Constructor Vacio
        public PhotoMessage(string sender) : base( sender) { } // Constructor que llama al constructor de la clase base
    }
}
