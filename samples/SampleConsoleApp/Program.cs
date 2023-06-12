using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetLah.Diagnostics;
using NetLah.Extensions.Configuration;
using NetLah.Extensions.Logging;

AppLog.InitLogger();

try
{
    AppLog.Logger.LogInformation("Application configure...");   // write log console only

    var configuration = ConfigurationBuilderBuilder.Create<Program>(args).Build();
    var logger = AppLog.CreateAppLogger<Program>(configuration);

    // write log to sinks

    var appInfo = ApplicationInfo.Initialize(null);
    logger.LogInformation("AppTitle:{appTitle}; Version:{appVersion} BuildTime:{appBuildTime}; Framework:{frameworkName}",
        appInfo.Title, appInfo.InformationalVersion, appInfo.BuildTimestampLocal, appInfo.FrameworkName);

    var asmConfigurationBinder = new AssemblyInfo(typeof(ConfigurationBinder).Assembly);
    logger.LogInformation("AssemblyTitle:{appTitle}; Version:{appVersion} Framework:{frameworkName}",
        asmConfigurationBinder.Title, asmConfigurationBinder.InformationalVersion, asmConfigurationBinder.FrameworkName);

    var asmLoggerFactory = new AssemblyInfo(typeof(LoggerFactory).Assembly);
    logger.LogInformation("AssemblyTitle:{appTitle}; Version:{appVersion} Framework:{frameworkName}",
        asmLoggerFactory.Title, asmLoggerFactory.InformationalVersion, asmLoggerFactory.FrameworkName);
}
catch (Exception ex)
{
    AppLog.Logger.LogCritical(ex, "Application terminated unexpectedly");
}
finally
{
    Serilog.Log.CloseAndFlush();
}
