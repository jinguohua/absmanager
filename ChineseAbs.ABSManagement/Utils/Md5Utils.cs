using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ChineseAbs.ABSManagement.Utils
{
    public static class Md5Utils
    {
        public static string Calc(Stream stream)
        {
            var bytesSource = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytesSource, 0, bytesSource.Length);

            var md5 = new MD5CryptoServiceProvider();
            var bytes = md5.ComputeHash(bytesSource);
            var stringBuilder = new StringBuilder();
            bytes.ToList().ForEach(x => stringBuilder.Append(x.ToString("x2")));
            return stringBuilder.ToString();
        }

        public static bool Equals(Stream left, Stream right)
        {
            var leftMD5 = Calc(left);
            var rightMD5 = Calc(right);
            return leftMD5 == rightMD5;
        }
    }
}
