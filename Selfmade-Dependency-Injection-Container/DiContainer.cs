using IServiceProvider = Selfmade_Dependency_Injection_Container.Interfaces.IServiceProvider;

namespace Selfmade_Dependency_Injection_Container;

[Obsolete("Please consider using ServiceCollection and ServiceProvider for SOLID based code", true)]
public class DiContainer : IServiceProvider, IDisposable
{
    private readonly Dictionary<Type, ServiceDescriptor> _services = new();
    private readonly Dictionary<Type, object> _singletonInstances = new(); // for singletons
    private readonly List<IDisposable> _disposables = new(); // for manageing disposables, important for scopes

    public DiContainer() { } // ctor for root-container

    //ctor for scopes
    private DiContainer(DiContainer parentContainer)
    {
        // Kopiere die Service-Definitionen vom Parent
        _services = parentContainer._services;
        // singletons will be managed by parent (root) continer
        _singletonInstances = parentContainer._singletonInstances;
    }

    private void AddService(Type serviceType, Type implementationType, Lifetime lifetime)
    {
        _services[serviceType] = new ServiceDescriptor(serviceType, implementationType, lifetime);
    }

    private void AddService(Type serviceType, Func<IServiceProvider, object> factory, Lifetime lifetime)
    {
        _services[serviceType] = new ServiceDescriptor(service: serviceType, factory: factory, lifetime: lifetime);
    }

    // --- AddTransient ---
    public void AddTransient<TService, TImplementation>() where TImplementation : TService
    {
        AddService(typeof(TService), typeof(TImplementation), Lifetime.Transient);
    }

    public void AddTransient<TService>() // Self-Registration (z.B. AddTransient<App>())
    {
        AddService(typeof(TService), typeof(TService), Lifetime.Transient);
    }

    public void AddTransient<TService>(Func<IServiceProvider, TService> factory) where TService : class
    {
        AddService(typeof(TService), (container) => factory(container)!, Lifetime.Transient);
    }

    // --- AddScoped ---
    public void AddScoped<TService, TImplementation>() where TImplementation : TService
    {
        AddService(typeof(TService), typeof(TImplementation), Lifetime.Scoped);
    }

    public void AddScoped<TService>() // Self-Registration
    {
        AddService(typeof(TService), typeof(TService), Lifetime.Scoped);
    }

    public void AddScoped<TService>(Func<IServiceProvider, TService> factory) where TService : class
    {
        AddService(typeof(TService), (container) => factory(container)!, Lifetime.Scoped);
    }

    // --- AddSingleton ---
    public void AddSingleton<TService, TImplementation>() where TImplementation : TService
    {
        AddService(typeof(TService), typeof(TImplementation), Lifetime.Singleton);
    }

    public void AddSingleton<TService>() // Self-Registration
    {
        AddService(typeof(TService), typeof(TService), Lifetime.Singleton);
    }

    public void AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class
    {
        AddService(typeof(TService), (container) => factory(container)!, Lifetime.Singleton);
    }

    public void AddSingleton<TService>(TService instance) where TService : class // registers an existing instanace
    {
        AddService(typeof(TService), (container) => instance, Lifetime.Singleton);
    }


    public TService GetService<TService>()
    {
        return (TService) Resolve(typeof(TService));
    }

    public object GetService(Type serviceType)
    {
        return Resolve(serviceType);
    }

     private object Resolve(Type serviceType)
    {
        if (!_services.TryGetValue(serviceType, out var descriptor))
            throw new Exception($"Service {serviceType.Name} not registered.");

        object implementation;

        
        // Singleton Handling: Search always in the Root-Continer (_singletonInstances)
        if (descriptor.Lifetime == Lifetime.Singleton)
        {
            if (_singletonInstances.TryGetValue(serviceType, out var instance))
                return instance;
            
        }
        // Scoped Handling: Nur im aktuellen Scope (_singletonInstances dieses DiContainer-Objekts) suchen
        else if (descriptor.Lifetime == Lifetime.Scoped)
        {
            if (_singletonInstances.TryGetValue(serviceType, out var scopedInstance)) 
                return scopedInstance;
            
        }

        // Instanziierung
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
            var parameterInstances = parameters.Select(p => Resolve(p.ParameterType)).ToArray();
            implementation = Activator.CreateInstance(actualType, parameterInstances)!;
        }

        // Cache und Dispose-Verwaltung
        if (descriptor.Lifetime == Lifetime.Singleton)
        {
            // Nur der Root-Container sollte Singleton-Instanzen cachen
            _singletonInstances[serviceType] = implementation;
        }
        else if (descriptor.Lifetime == Lifetime.Scoped)
        {
            // Scoped Instanzen werden im _singletonInstances des aktuellen Scopes gespeichert
            _singletonInstances[serviceType] = implementation;
            if (implementation is IDisposable disposable)
            {
                _disposables.Add(disposable);
            }
        }
        return implementation;
    }

    public DiContainer CreateScope()
    {
        var scope = new DiContainer(this);
        _disposables.Add(scope);
        return scope;
    }
    public void Dispose()
    {
        foreach (var disposable in _disposables.AsEnumerable().Reverse())
        {
            if (disposable != null)
            {
                try 
                { 
                    disposable.Dispose(); 
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