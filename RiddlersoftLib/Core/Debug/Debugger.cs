using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Riddlersoft.Core.Debug
{
    public static class Debugger
    {
        public static void WriteLine(string formattedText)
        {
#if DEBUG
            Console.WriteLine(formattedText);
#endif
        }

        public static void Write(string formattedText)
        {
#if DEBUG
            Console.WriteLine(formattedText);
#endif
        }

    }
}
