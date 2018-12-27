using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ChineseAbs.ABSManagement.Foundation
{
    public static class HashHelper
    {
        /// <summary>
        /// 获取字符串的MD5哈希值
        /// </summary>
        public static string GetMd5(string value, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.ASCII;
            }
            byte[] bytes = encoding.GetBytes(value);
            return GetMd5(bytes);
        }

        /// <summary>
        /// 获取字节数组的MD5哈希值
        /// </summary>
        public static string GetMd5(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            MD5 hash = new MD5CryptoServiceProvider();
            bytes = hash.ComputeHash(bytes);
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }

    public class Base64
    {
        public static string Encrypt(string plainText)
        {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bs);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] bs = Convert.FromBase64String(encryptedText);
            return System.Text.Encoding.UTF8.GetString(bs);
        }
    }
}
