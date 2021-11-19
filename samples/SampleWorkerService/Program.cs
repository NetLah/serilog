using NetLah.Extensions.Logging;
using SampleWorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .UseSerilog2(logger => logger.LogInformation("Application initializing..."))
    .Build();

await host.RunAsync();
