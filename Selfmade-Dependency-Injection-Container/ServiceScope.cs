using Selfmade_Dependency_Injection_Container.Interfaces;
using IServiceProvider = Selfmade_Dependency_Injection_Container.Interfaces.IServiceProvider;

namespace Selfmade_Dependency_Injection_Container;

/// <summary>
/// Implementation of IServiceScope. Encapsulates the actual provider for the scope.
/// </summary>
public class ServiceScope : IServiceScope
{
    public IServiceProvider ServiceProvider { get; }
    private readonly ServiceProvider _internalProvider; // The concrete Provider instance for this scope

    public ServiceScope(ServiceProvider parentProvider)
    {
        // Creates a new Provider that acts as a child of the parentProvider.
        // It inherits service definitions and refers to the root's singleton cache.
        _internalProvider = new ServiceProvider(parentProvider);
        ServiceProvider = _internalProvider;
    }

    public void Dispose()
    {
        _internalProvider.Dispose(); // Dispose the internal provider and thus all its scoped services
        GC.SuppressFinalize(this);
    }
}
