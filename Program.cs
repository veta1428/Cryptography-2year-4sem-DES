using System;
using System.Collections;
using System.Text;

namespace DES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string openText = @"123456782wudhwud uw uyduw duwoi2;u2   2y ydw dywd";
            byte[] data = Encoding.UTF8.GetBytes(openText);


            DES dES = new DES();
            //byte[] array = Encoding.UTF8.GetBytes("18191161");
            //BitArray[] keys = dES.KeySchedule(Encoding.UTF8.GetBytes("18191161"));
            //BitArray[] keys2 = dES.KeySchedule(Encoding.UTF8.GetBytes("18191161"));

            byte[] cr = dES.Encrypt(openText, "8191161", CryptoMode.ECB, AddMode.ISO_EIC);
            byte[] dr = dES.Decrypt(cr, "8191161", CryptoMode.ECB, AddMode.ISO_EIC);
            Console.WriteLine(Encoding.UTF8.GetString(dr));

            //BitArray openTextBits = new BitArray(Encoding.UTF8.GetBytes(openText));

            //BitArray crypted = dES.EncryptSingleBlock(keys, openTextBits);
            //BitArray decrypted = dES.DecryptSingleBlock(keys2, crypted);


            //for (int i = 0; i < crypted.Length; i++)
            //{
            //    if (openTextBits[i] != decrypted[i])
            //    {
            //        Console.WriteLine(i);
            //    }
            //}
        }
    }
}