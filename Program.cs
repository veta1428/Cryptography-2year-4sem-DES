using System.Text;

namespace DES
{
    public class Program
    {
        static void Main(string[] args)
        {
            string openText = @"12345678Hi!";
            string key = "8191Kl1";
            string iv = "12345678";
            AddMode addMode = AddMode.PKC;
            CryptoMode cryptoMode = CryptoMode.OFB;

            DES des = new DES();
            byte[] iv_bytes = Encoding.ASCII.GetBytes(iv);
            byte[] crypted = des.Encrypt(openText, key, cryptoMode, addMode, iv_bytes);
            byte[] decrypted = des.Decrypt(crypted, key, cryptoMode, addMode, iv_bytes);

            Console.WriteLine("Padding mode: " + addMode);
            Console.WriteLine("Crypting mode: " + cryptoMode);
            Console.WriteLine("Open text: " + openText);
            Console.WriteLine("Encrypted: " + Encoding.UTF8.GetString(crypted));
            Console.WriteLine("Decrypted: " + Encoding.UTF8.GetString(decrypted));
        }
    }
}