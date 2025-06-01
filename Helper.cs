using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BackendHtml
{
    public static class Helper
    {

        public static byte[] Hash(string plaintext)
        {
            using HashAlgorithm algorithm = SHA512.Create();
            return algorithm.ComputeHash(Encoding.ASCII.GetBytes(plaintext));
            // Encoding.ASCII.GetBytes(plaintext) trả về byte

        }

        public static int GetPage(this ViewContext context, string name)
        {
            object? obj = context.RouteData.Values[name];
            if (obj != null)
            {
                return Convert.ToInt32(obj);
            }
            return 1;
        }

        public static string RandomString(int len)
        {
            char[] arr = new char[len];
            Random rd = new Random();
            string pattern = "qwertyuioasdfghjklxcvbn1234789";
            for (int i = 0; i < len; i++)
            {
                arr[i] = pattern[rd.Next(pattern.Length)];
            }
            return string.Join("", arr);
        }

        public static int GetId(this ViewContext context, string name)
        {
            object? obj = context.RouteData.Values[name];
            if (obj != null)
            {
                return Convert.ToInt32(obj);
            }
            return 1;
        }
        public static string Summary(this string str, int len)
        {
            if (str.Length < len)
            {
                return str;
            }
            int index = str.IndexOf(' ', len);
            if (index > 0)
            {
                return str.Substring(0, index);
            }
            return str.Substring(0, len);
        }
    }
}