using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVS.CoreLib.ConsoleTools.Utils;

namespace AVS.CoreLib.SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var writer = new ConsoleWriter();
            
            FormattableString s1 = $"text before {DateTime.Now:d} {"arg1":Blue} integer {123:-f Yellow -b DarkGray} text after";
            writer.WriteLine(s1);
            FormattableString s2 = $"text before {DateTime.Now.ToString("d"):-f Blue -b Yellow} double {0.123:-b Green} text after";
            writer.WriteLine(s2);

            Console.ReadLine();
        }
    }
}
