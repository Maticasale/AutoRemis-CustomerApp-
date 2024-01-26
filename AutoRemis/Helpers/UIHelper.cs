using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AutoRemis.Helpers
{
    public static class UIHelper
    {
        public static string CapitalizeSentence(string text)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(TextoSinAcentos(text));
        }
        private static string TextoSinAcentos(string text)
        {
            //byte[] bytesISO = Encoding.GetEncoding("ISO-8859-1").GetBytes(text);
            //string textoSinAcentos = Encoding.UTF8.GetString(bytesISO, 0, bytesISO.Length);

            Encoding encoding = Encoding.GetEncoding("ISO-8859-1");
            string textoSinAcentos = encoding.GetString(encoding.GetBytes(text));

            string palabraSinAcento = textoSinAcentos
                .Replace("ã\u0081", "á")
                .Replace("ã¡", "á")
                .Replace("ã©", "é")
                .Replace("ã­", "í")
                .Replace("ã\"", "ó")
                .Replace("ã³", "ó")
                .Replace("ãº", "ú");

            return palabraSinAcento;
        }
    }
}
