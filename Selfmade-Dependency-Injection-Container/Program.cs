using Selfmade_Dependency_Injection_Container.Interfaces.Services;
using Selfmade_Dependency_Injection_Container.Interfaces;
using Selfmade_Dependency_Injection_Container.Services;
using Selfmade_Dependency_Injection_Container;

// 1. Register services in the ServiceCollection
// This phase is for configuration and collects service definitions.
IServiceCollection services = new ServiceCollection();
services.AddSingleton<ILogger, ConsoleLogger>();
services.AddScoped<IRepository, FileRepository>();
services.AddScoped<ServiceWithScopedDep>(); // Example of self-registration
services.AddTransient<App>(); // Example of self-registration
Console.WriteLine("Services registered.");

// 2. Build the ServiceProvider (Root Container)
// This is the point where the DI container is created
// and is ready for resolution and creating scopes.
// The Root ServiceProvider is IDisposable and should be disposed at the end of the application.
using (var serviceProvider = new ServiceProvider(services))
{
    Console.WriteLine("\nServiceProvider built. Entering first scope...");

    // 3. Usage in Scopes
    // Each 'using' block creates a new, isolated scope.
    // Services with 'Scoped' lifetime are created once per scope.
    using (var scope1 = serviceProvider.CreateScope())
    {
        Console.WriteLine("\n--- Scope 1 Started ---");
        // We use the ServiceProvider provided by the scope.
        var app1 = scope1.ServiceProvider.GetService<App>();
        app1.Run();

        Console.WriteLine("\nGetting IRepository in Scope 1 again...");
        var repo1_2 = scope1.ServiceProvider.GetService<IRepository>();
        Console.WriteLine($"Are IRepository instances the same in Scope 1? {ReferenceEquals(scope1.ServiceProvider.GetService<IRepository>(), repo1_2)}");

        Console.WriteLine("\n--- Scope 1 End ---");
    } // scope1.Dispose() is called here, disposing all scoped services in Scope 1

    Console.WriteLine("\nEntering second scope...");

    using (var scope2 = serviceProvider.CreateScope())
    {
        Console.WriteLine("\n--- Scope 2 Started ---");
        var app2 = scope2.ServiceProvider.GetService<App>();
        app2.Run();

        Console.WriteLine("\nGetting IRepository in Scope 2 again...");
        var repo2_2 = scope2.ServiceProvider.GetService<IRepository>();
        Console.WriteLine($"Are IRepository instances the same in Scope 2? {ReferenceEquals(scope2.ServiceProvider.GetService<IRepository>(), repo2_2)}");

        Console.WriteLine("\n--- Scope 2 End ---");
    } // scope2.Dispose() is called here, disposing all scoped services in Scope 2

    Console.WriteLine("\n--- Comparing instances across scopes ---");
    // Singletons should always be the same instance, regardless of where they are resolved.
    var loggerFromRoot1 = serviceProvider.GetService<ILogger>();
    var loggerFromRoot2 = serviceProvider.GetService<ILogger>();
    Console.WriteLine($"Are Singleton ILogger instances the same from root? {ReferenceEquals(loggerFromRoot1, loggerFromRoot2)}");

    Console.WriteLine("\nPress any key to exit.");
    Console.ReadKey();

} // serviceProvider.Dispose() is called here, disposing all singleton services and all tracked scopes.
        