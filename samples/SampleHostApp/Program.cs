using NetLah.Diagnostics;
using NetLah.Extensions.Logging;
using SampleHostApp;
using Serilog.Events;

AppLog.InitLogger(LogEventLevel.Information);

AppLog.Logger.LogInformation("Application starting...");

var appInfo = ApplicationInfo.TryInitialize(null);

var settings = new HostApplicationBuilderSettings
{
    Args = args
};

var builder = Host.CreateApplicationBuilder(settings);
builder.UseSerilog(logger => logger.LogInformation("Application initializing... AppTitle:{appTitle}; Version:{appVersion} BuildTime:{appBuildTime}; Framework:{frameworkName}",
    appInfo.Title, appInfo.InformationalVersion, appInfo.BuildTimestampLocal, appInfo.FrameworkName));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

var logger = AppLog.Logger;
var asmConfigurationBinder = new AssemblyInfo(typeof(ConfigurationBinder).Assembly);
logger.LogInformation("AssemblyTitle:{appTitle}; Version:{appVersion} Framework:{frameworkName}",
    asmConfigurationBinder.Title, asmConfigurationBinder.InformationalVersion, asmConfigurationBinder.FrameworkName);

var asmLoggerFactory = new AssemblyInfo(typeof(LoggerFactory).Assembly);
logger.LogInformation("AssemblyTitle:{appTitle}; Version:{appVersion} Framework:{frameworkName}",
    asmLoggerFactory.Title, asmLoggerFactory.InformationalVersion, asmLoggerFactory.FrameworkName);

await host.RunAsync();
