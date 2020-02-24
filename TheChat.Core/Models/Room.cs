using System;
using System.Collections.Generic;
using System.Text;

namespace TheChat.Core.Models
{
    /// <summary>
    /// Modelo que contiene Nombre, Imagen y # de usuarios de la sal
    /// </summary>
    public class Room
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public int UsersNumber { get; set; }
    }
}
