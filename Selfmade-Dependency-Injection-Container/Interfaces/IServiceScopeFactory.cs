using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfmade_Dependency_Injection_Container.Interfaces
{
    /// <summary>
    /// Interface for creating new service scopes.
    /// </summary>
    public interface IServiceScopeFactory
    {
        IServiceScope CreateScope();
    }
}
