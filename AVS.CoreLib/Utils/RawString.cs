using System;

namespace AVS.CoreLib.Utils
{
    public class RawString
    {
        public string Value { get; }

        private RawString(string str)
        {
            Value = str;
        }

        public static implicit operator RawString(string str)
        {
            return new RawString(str);
        }

        public static implicit operator RawString(FormattableString formattable)
        {
            return new RawString(formattable.ToString());
        }

        public static implicit operator string(RawString str)
        {
            return str?.Value;
        }
    }
}