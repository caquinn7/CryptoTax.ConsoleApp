using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoTaxV3.Domain
{
    public static class Extensions
    {
        // string

        public static string Capitalize(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            char[] chars = input.ToCharArray();
            chars[0] = char.ToUpper(input[0]);
            return new string(chars);
        }

        public static string ToSHA256(this string input, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = hmac.ComputeHash(inputBytes);
            return hashBytes.ToHexString();
        }

        public static string ToMD5(this string input)
        {
            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return hashBytes.ToHexString();
        }

        public static string TrimAndNullIfEmpty(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            return input.Trim();
        }

        // byte[]

        public static string ToHexString(this byte[] bytes) =>
            string.Concat(bytes.Select(b => b.ToString("x2")));

        // long

        public static DateTime ToDateTime(this long input) =>
            DateTimeOffset.FromUnixTimeSeconds(input).DateTime;

        public static DateTime? ToDateTime(this long? input) =>
            input != null ? DateTimeOffset.FromUnixTimeSeconds(input.Value).DateTime : null;

        // DateTime

        /// <summary>
        /// Assumes DateTime object is Local Time
        /// </summary>
        public static long ToUnixSeconds(this DateTime input) =>
            ((DateTimeOffset)input).ToUnixTimeSeconds();

        /// <summary>
        /// Assumes DateTime object is Local Time
        /// </summary>
        public static long? ToUnixSeconds(this DateTime? input) =>
            input != null ? ((DateTimeOffset)input.Value).ToUnixTimeSeconds() : null;
    }
}
