using System;
using System.Collections;
using System.Text;

namespace DES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string openText = @"12345edejdhejde eyfe fye fry rf urОЛЦЫВЦЛЦЛОЫВЦ6789";

            DES dES = new DES();
            byte[] cr = dES.Encrypt(openText, "8191161", CryptoMode.OFB, AddMode.ISO_EIC, Encoding.ASCII.GetBytes("12345678"));
            byte[] dr = dES.Decrypt(cr, "8191161", CryptoMode.OFB, AddMode.ISO_EIC, Encoding.ASCII.GetBytes("12345678"));
            Console.WriteLine(Encoding.UTF8.GetString(dr));
        }
    }
}