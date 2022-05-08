using System;
using System.Collections;
using System.Text;

namespace DES
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string openText = "12345668";
            byte[] data = Encoding.UTF8.GetBytes(openText);


            DES dES = new DES();
            byte[] array = Encoding.UTF8.GetBytes("18191161");
            BitArray[] keys = dES.KeySchedule(Encoding.UTF8.GetBytes("18191161"));
            BitArray[] keys2 = dES.KeySchedule(Encoding.UTF8.GetBytes("18191161"));

            BitArray openTextBits = new BitArray(Encoding.UTF8.GetBytes(openText));

            BitArray crypted = dES.EncryptSingleBlock(keys, openTextBits);
            BitArray decrypted = dES.DecryptSingleBlock(keys2, crypted);


            for (int i = 0; i < crypted.Length; i++)
            {
                if (openTextBits[i] != decrypted[i])
                {
                    Console.WriteLine(i);
                }
            }

            //dES.EncryptSingleBlock(data, );

            //BitArray bits4 = new BitArray(4);

            //int number = 10;
            //bits4[3] = Convert.ToBoolean(number % 2);
            //number /= 2;
            //bits4[2] = Convert.ToBoolean(number % 2);
            //number /= 2;
            //bits4[1] = Convert.ToBoolean(number % 2);
            //number /= 2;
            //bits4[0] = Convert.ToBoolean(number % 2);

            //for (int i = 0; i < bits4.Count; i++)
            //{
            //    Console.WriteLine(bits4[i]);
            //}

        }
    }
}