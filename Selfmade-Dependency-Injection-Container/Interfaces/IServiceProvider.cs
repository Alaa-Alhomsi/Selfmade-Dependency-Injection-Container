namespace Selfmade_Dependency_Injection_Container.Interfaces;

/// <summary>
/// Interface for resolving services.
/// This represents the "Runtime" phase of the DI container.
/// </summary>
public interface IServiceProvider : IDisposable // ServiceProvider instances can be disposed (e.g., scopes)
{
    TService GetService<TService>();
    object GetService(Type serviceType);
}
