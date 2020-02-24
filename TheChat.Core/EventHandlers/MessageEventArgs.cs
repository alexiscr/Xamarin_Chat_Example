using System;
using System.Collections.Generic;
using System.Text;
using TheChatMessages;

namespace TheChat.Core.EventHandlers
{
    /// <summary>
    /// Manejador de enventos para notificar el envio de mensajes
    /// </summary>
    public class MessageEventArgs
    {
        public ChatMessage Message { get; set; }

        public MessageEventArgs(ChatMessage message)
        {
            Message = message;
        }
    }
}
