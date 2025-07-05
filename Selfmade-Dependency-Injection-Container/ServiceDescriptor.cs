using IServiceProvider = Selfmade_Dependency_Injection_Container.Interfaces.IServiceProvider;

namespace Selfmade_Dependency_Injection_Container;

/// <summary>
/// Defines the lifetime of a service within the DI container.
/// </summary>
public enum Lifetime
{
    Transient,
    Singleton,
    Scoped
}

/// <summary>
/// Describes a registered service: its type, its implementation, lifetime, and an optional factory.
/// </summary>
public class ServiceDescriptor
{
    public Type ServiceType { get; }
    public Type ImplementationType { get; }
    public Lifetime Lifetime { get; }
    public Func<IServiceProvider, object>? Factory { get; }

    /// <summary>
    /// Constructor for type-based service registrations (e.g., AddTransient<IService, ServiceImpl>()).
    /// </summary>
    /// <param name="service">The service type (often an interface).</param>
    /// <param name="impl">The implementation type.</param>
    /// <param name="lifetime">The lifetime of the service.</param>
    public ServiceDescriptor(Type service, Type impl, Lifetime lifetime)
    {
        ServiceType = service;
        ImplementationType = impl;
        Lifetime = lifetime;
        Factory = null;
    }
    
    /// <summary>
    /// Constructor for factory-based service registrations (e.g., AddSingleton<IService>(sp => new ServiceImpl(sp.GetService<IDep>()))).
    /// </summary>
    /// <param name="service">The service type.</param>
    /// <param name="factory">The factory function that creates an instance of the service.</param>
    /// <param name="lifetime">The lifetime of the service.</param>
    public ServiceDescriptor(Type service, Func<IServiceProvider, object> factory, Lifetime lifetime)
    {
        ServiceType = service;
        ImplementationType = service; // ImplementationType often irrelevant for factories, set to ServiceType
        Lifetime = lifetime;
        Factory = factory;
    }
}

