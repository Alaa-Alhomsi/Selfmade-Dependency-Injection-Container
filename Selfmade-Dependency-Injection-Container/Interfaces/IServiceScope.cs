using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selfmade_Dependency_Injection_Container.Interfaces
{
    /// <summary>
    /// Represents a service scope, which provides its own IServiceProvider and is disposable.
    /// </summary>
    public interface IServiceScope : IDisposable
    {
        IServiceProvider ServiceProvider { get; }
    }
}
