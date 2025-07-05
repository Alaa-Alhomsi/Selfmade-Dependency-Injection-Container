using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfmade_Dependency_Injection_Container.Interfaces.Services
{
    public interface IRepository : IDisposable
    {
        void Save(string data);
    }
}
