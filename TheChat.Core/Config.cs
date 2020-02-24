using System;
using System.Collections.Generic;
using System.Text;

namespace TheChat.Core
{

    // Clase que contiene los Endpoints del servicio de Azure SignalR
    public static class Config
    {
        // Endpoint principal
        public static string MainEndpoint = "http://localhost:7071";    
        
        // Endpoint que establecera la conexión inicial con el servicio de SignalR
        public static string NegotiateEndpoint = $"{MainEndpoint}/api/negotiate"; 

        // Endpoint para el envio de mensaje
        public static string MessagesEndpoint = $"{MainEndpoint}/api/Messages";

        // Endpoint para agregar el usuario al grupo seleccionado
        public static string AddToGroupEndPoint = $"{MainEndpoint}/api/AddToGroup";

        // Endpoint para remover un usuario del grupo actual
        public static string LeaveGroupEndpoint = $"{MainEndpoint}/api/RemoveFromGroup";

        // Endpoint para obtener una lista de usuarios
        public static string RoomsEndpoint = $"{MainEndpoint}/api/Users";

        // Endpit para obtener un usuario en especifico
        public static string UserEndpoint = $"{MainEndpoint}/api/User";
    }
}
