using NetLah.Diagnostics;
using NetLah.Extensions.Logging;
using SampleWorkerService;

var appInfo = ApplicationInfo.TryInitialize(null);

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .UseSerilog2(logger => logger.LogInformation("Application initializing... AppTitle:{appTitle}; Version:{appVersion} BuildTime:{appBuildTime}; Framework:{frameworkName}",
        appInfo.Title, appInfo.InformationalVersion, appInfo.BuildTimestampLocal, appInfo.FrameworkName))
    .Build();

await host.RunAsync();
