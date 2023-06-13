using NetLah.Diagnostics;
using NetLah.Extensions.Logging;
using SampleWorkerService;
using Serilog.Events;

AppLog.InitLogger(LogEventLevel.Information);

AppLog.Logger.LogInformation("Application starting...");

var appInfo = ApplicationInfo.TryInitialize(null);

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .UseSerilog2(logger => logger.LogInformation("Application initializing... AppTitle:{appTitle}; Version:{appVersion} BuildTime:{appBuildTime}; Framework:{frameworkName}",
        appInfo.Title, appInfo.InformationalVersion, appInfo.BuildTimestampLocal, appInfo.FrameworkName));

IHost host = hostBuilder.Build();

var logger = AppLog.Logger;
var asmConfigurationBinder = new AssemblyInfo(typeof(ConfigurationBinder).Assembly);
logger.LogInformation("AssemblyTitle:{appTitle}; Version:{appVersion} Framework:{frameworkName}",
    asmConfigurationBinder.Title, asmConfigurationBinder.InformationalVersion, asmConfigurationBinder.FrameworkName);

var asmLoggerFactory = new AssemblyInfo(typeof(LoggerFactory).Assembly);
logger.LogInformation("AssemblyTitle:{appTitle}; Version:{appVersion} Framework:{frameworkName}",
    asmLoggerFactory.Title, asmLoggerFactory.InformationalVersion, asmLoggerFactory.FrameworkName);

await host.RunAsync();
