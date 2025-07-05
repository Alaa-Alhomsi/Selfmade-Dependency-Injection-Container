using Selfmade_Dependency_Injection_Container.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfmade_Dependency_Injection_Container.Services
{
    public class ServiceWithScopedDep
    {
        public ServiceWithScopedDep(IRepository repo)
        {
            Console.WriteLine("ServiceWithScopedDep created");
        }
        public void DoSomething()
        {
            Console.WriteLine("DoSomething-Method called");
        }
    }
}
