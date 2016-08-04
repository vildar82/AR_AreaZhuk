using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR_Zhuk_Scheme_ConsoleTest.Scheme;

namespace AR_Zhuk_Scheme_ConsoleTest
{
    class Program
    {
        static void Main (string[] args)
        {
            TextWriterTraceListener writer = new TextWriterTraceListener(Console.Out);
            Debug.Listeners.Add(writer);

            TestProjectScheme test = new TestProjectScheme();
            test.TestTotalHouses();

            Console.ReadKey();
        }
    }
}
