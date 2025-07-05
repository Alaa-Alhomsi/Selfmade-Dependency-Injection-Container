using Selfmade_Dependency_Injection_Container.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfmade_Dependency_Injection_Container.Services
{
    public class ConsoleLogger : ILogger
    {

        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Dispose()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("ConsoleLogger disposed.");
            Console.ResetColor();
        }
    }
}
