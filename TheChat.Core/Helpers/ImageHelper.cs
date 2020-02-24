using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheChat.Core.Helpers
{
    /// <summary>
    /// Helper para la conversion de imagenes a bytes
    /// </summary>
    public static class ImageHelper
    {
        public static byte[] ReadFully(this Stream input)
        {
            byte[] buffer = new byte[16 * 1024];

            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
