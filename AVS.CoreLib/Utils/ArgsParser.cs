using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.Utils
{
    public class ArgsParser
    {
        private const string ARGS_REGEX = @"-((?<param>(\w)+) (?<arg>(\w|\S)+))";

        public static Dictionary<string, string> Parse(string args)
        {
            var dict = new Dictionary<string, string>();
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
    }
}