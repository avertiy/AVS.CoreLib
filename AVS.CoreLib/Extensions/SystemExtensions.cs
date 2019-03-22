using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.CoreLib.Extensions
{
    public static class SystemExtensions
    {
        public static string Truncate(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static string ToStringHex(this byte[] value)
        {
            var output = string.Empty;
            for (var i = 0; i < value.Length; i++)
            {
                output += value[i].ToString("x2", CultureInfo.InvariantCulture);
            }
            return (output);
        }

        public static Type FindGenericType(this Type type, string name)
        {
            if (type.IsGenericType)
            {
                if (type.Name.StartsWith(name))
                    return type;
                if (type.BaseType != null)
                    return type.BaseType.FindGenericType(name);
            }
            return null;
        }
    }
}
