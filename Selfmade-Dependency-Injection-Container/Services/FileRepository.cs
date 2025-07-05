using Selfmade_Dependency_Injection_Container.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfmade_Dependency_Injection_Container.Services
{
    public class FileRepository : IRepository
    {
        public void Save(string data)
        {
            Console.WriteLine("File Saved");
        }

        public void Dispose()
        {
            //
        }

    }
}
