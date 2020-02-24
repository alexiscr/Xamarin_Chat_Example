using System;
using System.Collections.Generic;
using System.Text;

namespace TheChat.Core.Models
{
    /// <summary>
    /// Modelo de la clase usuario
    /// </summary>
    public class User
    {
        public string UserId { get; set; }
        public string Room { get; set; }
        public string Color { get; set; }
        public string Avatar { get; set; }
    }
}
