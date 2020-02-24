using System;
using System.Collections.Generic;
using System.Text;

namespace TheChat.Core.Models
{
    /// <summary>
    /// Modelo que contiene Url y el token de acceso al servicio
    /// </summary>
    public class ConnectionInfo
    {
        public string Url { get; set; }
        public string AccessToken { get; set; }
    }

}
