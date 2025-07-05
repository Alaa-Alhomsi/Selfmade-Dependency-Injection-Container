using Selfmade_Dependency_Injection_Container.Interfaces.Services;
using Selfmade_Dependency_Injection_Container.Services;

namespace Selfmade_Dependency_Injection_Container;

public class App
{
    private readonly IRepository _repository;
    private readonly ILogger _logger;
    private readonly ServiceWithScopedDep _scopedService;

    public App(IRepository repository, ILogger logger, ServiceWithScopedDep scopedService)
    {
        _repository = repository;
        _logger = logger;
        _scopedService = scopedService;
        Console.WriteLine("App instance created.");
    }

    public void Run()
    {
        _logger.Log("App is running.");
        _repository.Save("Hello World from App!");
        _scopedService.DoSomething();
    }
}
