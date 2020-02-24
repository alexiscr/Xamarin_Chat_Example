using System;
using System.Collections.Generic;
using System.Text;

namespace TheChatMessages
{
    // Clase para gestionar los mensajes que el usuario local envia
    public class LocalSimpleTextMessage : SimpleTextMessage
    {
        public LocalSimpleTextMessage(SimpleTextMessage message)
        {
            Id = message.Id;
            Sender = message.Sender;
            TypeInfo = message.TypeInfo;
            Timestamp = message.Timestamp;
            Recipient = message.Recipient;
            IsPrivate = message.IsPrivate;
            GroupName = message.GroupName;
            Text = message.Text;
        }
    }
}
