using Selfmade_Dependency_Injection_Container.Interfaces;
using IServiceProvider = Selfmade_Dependency_Injection_Container.Interfaces.IServiceProvider;

namespace Selfmade_Dependency_Injection_Container;

/// <summary>
/// The central service provider that resolves services and creates scopes.
/// Implements IServiceProvider for resolution and IServiceScopeFactory for scopes.
/// </summary>
public class ServiceProvider : IServiceProvider, IServiceScopeFactory
{
    private readonly Dictionary<Type, ServiceDescriptor> _serviceDescriptors;
    private readonly Dictionary<Type, object> _instanceCache = new(); // Cache for Singleton (in Root) and Scoped (in Scope) instances
    private readonly List<IDisposable> _disposables = new(); // List of IDisposable instances created by this provider
    private readonly ServiceProvider? _parentProvider; // Reference to the parent provider for Singleton resolution

    /// <summary>
    /// Constructor for the Root ServiceProvider.
    /// Takes an IServiceCollection to get the service definitions.
    /// </summary>
    /// <param name="services">The collected service definitions.</param>
    public ServiceProvider(IServiceCollection services)
    {
        _serviceDescriptors = services.ToDictionary(s => s.ServiceType);
        _parentProvider = null; // The root provider has no parent
    }

    /// <summary>
    /// Internal constructor for child ServiceProviders (scopes).
    /// </summary>
    /// <param name="parentProvider">The parent ServiceProvider (the root provider or another scope).</param>
    internal ServiceProvider(ServiceProvider parentProvider) 
    {
        _serviceDescriptors = parentProvider._serviceDescriptors; // Scopes share the service definitions of the Root Provider
        _parentProvider = parentProvider; // Reference to the parent for Singleton resolution
    }

    public TService GetService<TService>()
    {
        return (TService)GetService(typeof(TService));
    }

    public object GetService(Type serviceType)
    {
        return Resolve(serviceType);
    }

    /// <summary>
    /// Creates a new service scope.
    /// </summary>
    /// <returns>An IServiceScope instance.</returns>
    public IServiceScope CreateScope()
    {
        var scope = new ServiceScope(this);
        _disposables.Add(scope); // The Root Provider (or parent scope) manages the disposal of its child scopes
        return scope;
    }

    /// <summary>
    /// The core resolution logic for services.
    /// </summary>
    private object Resolve(Type serviceType)
    {
        if (!_serviceDescriptors.TryGetValue(serviceType, out var descriptor))
            throw new InvalidOperationException($"Service {serviceType.Name} not registered.");

        // Prevent resolving scoped services from the root provider
        if (descriptor.Lifetime == Lifetime.Scoped && _parentProvider == null)
        {
            throw new InvalidOperationException(
                $"Cannot resolve scoped service '{serviceType.Name}' from root provider. " +
                "Scoped services must be resolved from a service scope.");
        }

        object implementation;

        // Singleton Handling: Singletons are always managed and cached by the the root provider.
        if (descriptor.Lifetime == Lifetime.Singleton)
        {
            // If not the root, delegate singleton resolution to the parent.
            if (_parentProvider != null)
            {
                return _parentProvider.GetService(serviceType);
            }
            // If root, check current cache.
            else if (_instanceCache.TryGetValue(serviceType, out var singletonInstance))
            {
                return singletonInstance;
            }
        }
        // Scoped Handling: Scoped instances are cached within the current provider's (scope's) cache.
        else if (descriptor.Lifetime == Lifetime.Scoped)
        {
            if (_instanceCache.TryGetValue(serviceType, out var scopedInstance))
            {
                return scopedInstance;
            }
        }

        // Service Instantiation
        if (descriptor.Factory != null)
        {
            implementation = descriptor.Factory(this);
        }
        else
        {
            var actualType = descriptor.ImplementationType;
            var constructor = actualType.GetConstructors().FirstOrDefault();

            if (constructor == null)
            {
                throw new InvalidOperationException($"No public constructor found for type {actualType.Name}.");
            }

            var parameters = constructor.GetParameters();
            var parameterInstances = parameters.Select(p => GetService(p.ParameterType)).ToArray();
            implementation = Activator.CreateInstance(actualType, parameterInstances)!;
        }

        // Cache and Disposal Management based on lifetime
        if (descriptor.Lifetime == Lifetime.Singleton)
        {
            // Only the root provider stores singletons in its cache.
            if (_parentProvider == null)
            {
                _instanceCache[serviceType] = implementation;
            }
        }
        else if (descriptor.Lifetime == Lifetime.Scoped)
        {
            // Scoped instances are stored in the current provider's (scope's) cache.
            _instanceCache[serviceType] = implementation;
            // Add disposable services to the list.
            if (implementation is IDisposable disposable)
            {
                _disposables.Add(disposable);
            }
        }
        // Transient services are not cached.

        return implementation;
    }

    /// <summary>
    /// Disposes all IDisposable instances created within this ServiceProvider or its child scopes.
    /// </summary>
    public void Dispose()
    {
        // Dispose in reverse order to handle dependencies correctly
        foreach (var disposable in _disposables.AsEnumerable().Reverse())
        {
            if (disposable != null)
            {
                try
                {
                    disposable.Dispose();
                    Console.WriteLine($"Disposed: {disposable.GetType().Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error disposing {disposable.GetType().Name}: {ex.Message}");
                }
            }
        }
        _disposables.Clear();
        GC.SuppressFinalize(this);
    }
}
