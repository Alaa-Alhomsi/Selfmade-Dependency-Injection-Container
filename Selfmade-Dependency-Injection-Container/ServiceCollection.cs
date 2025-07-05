using Selfmade_Dependency_Injection_Container.Interfaces;
using System.Collections;
using IServiceProvider = Selfmade_Dependency_Injection_Container.Interfaces.IServiceProvider;

namespace Selfmade_Dependency_Injection_Container;



/// <summary>
/// Implementation of IServiceCollection for collecting service descriptors.
/// </summary>
public class ServiceCollection : IServiceCollection
{
    private readonly List<ServiceDescriptor> _descriptors = new();

    public IServiceCollection AddTransient<TService, TImplementation>() where TImplementation : TService
    { 
        AddService(typeof(TService), typeof(TImplementation), Lifetime.Transient); 
        
        return this; 
    }
    public IServiceCollection AddTransient<TService>() where TService : class
    { 
        AddService(typeof(TService), typeof(TService), Lifetime.Transient); 

        return this; 
    }
    public IServiceCollection AddTransient<TService>(Func<IServiceProvider, TService> factory) where TService : class
    { 
        AddService(typeof(TService), (sp) => factory(sp)!, Lifetime.Transient); 
        
        return this; 
    }

    public IServiceCollection AddScoped<TService, TImplementation>() where TImplementation : TService
    { 
        AddService(typeof(TService), typeof(TImplementation), Lifetime.Scoped); 
        
        return this; 
    }
    public IServiceCollection AddScoped<TService>() where TService : class
    { 
        AddService(typeof(TService), typeof(TService), Lifetime.Scoped); 
        
        return this; 
    }
    public IServiceCollection AddScoped<TService>(Func<IServiceProvider, TService> factory) where TService : class
    { 
        AddService(typeof(TService), (sp) => factory(sp)!, Lifetime.Scoped); 
        
        return this; 
    }

    public IServiceCollection AddSingleton<TService, TImplementation>() where TImplementation : TService
    { 
        AddService(typeof(TService), typeof(TImplementation), Lifetime.Singleton); 
        
        return this; 
    }
    public IServiceCollection AddSingleton<TService>() where TService : class
    { 
        AddService(typeof(TService), typeof(TService), Lifetime.Singleton); 
        
        return this; 
    }
    public IServiceCollection AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class
    { 
        AddService(typeof(TService), (sp) => factory(sp)!, Lifetime.Singleton); 
        
        return this; 
    }
    public IServiceCollection AddSingleton<TService>(TService instance) where TService : class
    { 
        AddService(typeof(TService), (sp) => instance!, Lifetime.Singleton); 
        
        return this; 
    }


    private void AddService(Type serviceType, Type implementationType, Lifetime lifetime)
    {
        _descriptors.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
    }

    private void AddService(Type serviceType, Func<IServiceProvider, object> factory, Lifetime lifetime)
    {
        _descriptors.Add(new ServiceDescriptor(serviceType, factory, lifetime));
    }

    public IEnumerator<ServiceDescriptor> GetEnumerator() => _descriptors.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    
}
