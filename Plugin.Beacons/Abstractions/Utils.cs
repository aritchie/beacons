using System;
using System.Globalization;
using System.Linq;


namespace Plugin.Beacons
{
    public static class Utils
    {
        public static bool IsEmpty(this string @string) => String.IsNullOrWhiteSpace(@string);


        public static string FromHexUuid(this string value)
        {
            var raw = value.Replace("-", String.Empty);
            var uuid = Guid.Parse(raw);
            return uuid.ToString();
        }


        public static bool TryFromHexUuid(this string value, out string uuid)
        {
            uuid = null;
            var raw = value.Replace("-", String.Empty);

            if (Guid.TryParse(raw, out var guid))
            {
                uuid = guid.ToString();
                return true;
            }
            return false;
        }


        public static byte[] FromHex(this string hex)
        {
            hex = hex
                .Replace("-", String.Empty)
                .Replace(" ", String.Empty);

            if (hex.Length % 2 != 0)
                throw new ArgumentException("Invalid hex string");

            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var value = hex.Substring(i * 2, 2);
                bytes[i] = Byte.Parse(value, NumberStyles.HexNumber);
            }
            return bytes;
        }


        public static string ToHexString(this byte[] bytes) => String.Concat(bytes.Select(b => b.ToString("X2")));
    }
}