using System;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization.Json;
using System.IO;

namespace ChineseAbs.ABSManagement.Utils
{
    public struct RsaKeys
    {
        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }

    public class RsaUtils
    {
        public static string Encrypt(object obj)
        {
            var publicKey = WebConfigUtils.RsaPublicKey;
            return Encrypt(publicKey, obj);
        }

        public static T Decrypt<T>(string text)
        {
            var privateKey = WebConfigUtils.RsaPrivateKey;
            return Decrypt<T>(privateKey, text);
        }

        public static string Encrypt(string publicKey, object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            var json = Encoding.UTF8.GetString(dataBytes);

            return Encrypt(publicKey, json);
        }

        public static T Decrypt<T>(string privateKey, string text)
        {
            var json = Decrypt(privateKey, text);

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return (T)serializer.ReadObject(stream);
        }

        public static string Encrypt(string publicKey, string text)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publicKey);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(text), false);

            return Convert.ToBase64String(cipherbytes);
        }

        public static string Decrypt(string privateKey, string text)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(privateKey);
            cipherbytes = rsa.Decrypt(Convert.FromBase64String(text), false);

            return Encoding.UTF8.GetString(cipherbytes);
        }

        public static RsaKeys GenerateRsaKeys()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RsaKeys keys = new RsaKeys()
            {
                PublicKey = rsa.ToXmlString(false),
                PrivateKey = rsa.ToXmlString(true)
            };

            return keys;
        }
    }
}
