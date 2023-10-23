using System.Security.Cryptography;
using System.Text;
using System;

namespace ProjetoIBGE.Configuration
{
    public static class Configuracao
    {
    
        public static string Key = "ZmVkYWY3ZDg4NjNiNDhlMTk3YjkyODdkNDkyYjcwOGU=";
    
        public static string Decrypt(string cipherText, string privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                byte[] encryptedData = Convert.FromBase64String(cipherText);
                byte[] decryptedData = rsa.Decrypt(encryptedData, false);
                return Encoding.UTF8.GetString(decryptedData);
            }
        }
    }
}
