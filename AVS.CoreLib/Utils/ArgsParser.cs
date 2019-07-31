using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.Utils
{
    public class ArgsParser
    {
        private const string ARGS_REGEX = @"-((?<param>(\w)+) (?<arg>(\w|\S)+))";

        /// <summary>
        /// example: -p value1 -x val/complex2
        /// </summary>
        public static Dictionary<string, string> Parse(string args)
        {
            var dict = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(args))
                return dict;
            
            var results = Regex.Matches(args, ARGS_REGEX, RegexOptions.Compiled);
            foreach (Match match in results)
            {
                string key = null;
                var gr = match.Groups["param"];
                if (gr.Success)
                {
                    key = gr.Value;
                }
                gr = match.Groups["arg"];
                if (gr.Success && !string.IsNullOrEmpty(key))
                {
                    dict.Add(key,gr.Value);
                }
            }

            return dict;
        }

        public static T Parse<T>(string args) where T: IDictionary<string, string>, new()
        {
            var dict = new T();
            if (string.IsNullOrEmpty(args))
                return dict;

            var results = Regex.Matches(args, ARGS_REGEX, RegexOptions.Compiled);
            foreach (Match match in results)
            {
                string key = null;
                var gr = match.Groups["param"];
                if (gr.Success)
                {
                    key = gr.Value;
                }
                gr = match.Groups["arg"];
                if (gr.Success && !string.IsNullOrEmpty(key))
                {
                    dict.Add(key, gr.Value);
                }
            }

            return dict;
        }
    }

    public interface IArgsParser
    {
        T Parse<T>(string args) where T: IDictionary<string, string>;
    }
}