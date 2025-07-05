namespace Selfmade_Dependency_Injection_Container.Interfaces.Services;

public interface ILogger : IDisposable
{
    void Log(string message);
}
