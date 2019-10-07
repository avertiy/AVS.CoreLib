using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.ConsoleTools.Utils
{
    /// <summary>
    /// parses formatted string int text: color scheme 
    /// </summary>
    public class ConsoleFormattedString: IEnumerable<(string, ColorScheme, string)>
    {
        private static readonly Regex Regex = new Regex(@"\$\$(?<scheme>-.*?){{(?<text>.*?)}}", RegexOptions.Compiled);
        public string Value { get; }

        private ConsoleFormattedString(string str)
        {
            Value = str;
        }

        private IEnumerable<(string, ColorScheme, string)> Iterate()
        {
            var match = Regex.Match(Value);
            var pos = 0;
            
            while (match.Success)
            {
                var plainText = Value.Substring(pos, match.Index - pos);
                pos = match.Index + match.Length;
                
                var schemeStr = match.Groups["scheme"].Value;
                var text = match.Groups["text"].Value;
                if (ColorScheme.TryParse(schemeStr, out ColorScheme scheme))
                {
                    yield return (plainText, scheme, text);
                }
                else
                {
                    yield return (plainText, null, text);
                }
                match = match.NextMatch();
            }

            if (pos < Value.Length)
            {
                var restText = Value.Substring(pos);
                yield return (restText, null, null);
            }
        }
        
        public IEnumerator<(string, ColorScheme, string)> GetEnumerator()
        {
            return Iterate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Iterate().GetEnumerator();
        }

        public static explicit operator ConsoleFormattedString(string str)
        {
            return new ConsoleFormattedString(str);
        }

        public static implicit operator ConsoleFormattedString(FormattableString str)
        {
            return new ConsoleFormattedString(str.ToString(ConsoleFormatProvider.Instance));
        }

        public static implicit operator string(ConsoleFormattedString str)
        {
            return str?.Value;
        }
    }
}