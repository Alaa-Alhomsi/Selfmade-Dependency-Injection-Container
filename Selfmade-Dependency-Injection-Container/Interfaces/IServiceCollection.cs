using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IServiceProvider = Selfmade_Dependency_Injection_Container.Interfaces.IServiceProvider;

namespace Selfmade_Dependency_Injection_Container.Interfaces
{
    /// <summary>
    /// Interface for registering services.
    /// This represents the "Configuration" phase of the DI container.
    /// </summary>
    public interface IServiceCollection : IEnumerable<ServiceDescriptor>
    {
        // Fluent API: Methods return IServiceCollection to enable chaining.
        IServiceCollection AddTransient<TService, TImplementation>() where TImplementation : TService;
        IServiceCollection AddTransient<TService>() where TService : class;
        IServiceCollection AddTransient<TService>(Func<IServiceProvider, TService> factory) where TService : class;

        IServiceCollection AddScoped<TService, TImplementation>() where TImplementation : TService;
        IServiceCollection AddScoped<TService>() where TService : class;
        IServiceCollection AddScoped<TService>(Func<IServiceProvider, TService> factory) where TService : class;

        IServiceCollection AddSingleton<TService, TImplementation>() where TImplementation : TService;
        IServiceCollection AddSingleton<TService>() where TService : class;
        IServiceCollection AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class;
        IServiceCollection AddSingleton<TService>(TService instance) where TService : class;
    }
}
