﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetLah.Diagnostics;
using NetLah.Extensions.Configuration;
using NetLah.Extensions.Logging;
using SampleConsoleAppDependencyInjection;

AppLog.InitLogger();

try
{
    AppLog.Logger.LogInformation("Application configure...");   // write log console only

    var configBuilder = ConfigurationBuilderBuilder.Create<Program>(args);
#if NET6_0_OR_GREATER
    var configuration = configBuilder.Manager;
#else
    var configuration = configBuilder.Build();
#endif

    var logger = AppLog.CreateAppLogger<Program>(configuration);

    // write log to sinks

    var appInfo = ApplicationInfo.Initialize(null);
    logger.LogInformation("Application initializing... AppTitle:{appTitle}; Version:{appVersion} BuildTime:{appBuildTime}; Framework:{frameworkName}",
        appInfo.Title, appInfo.InformationalVersion, appInfo.BuildTimestampLocal, appInfo.FrameworkName);

    var asmConfigurationBinder = new AssemblyInfo(typeof(ConfigurationBinder).Assembly);
    logger.LogInformation("AssemblyTitle:{appTitle}; Version:{appVersion} Framework:{frameworkName}",
        asmConfigurationBinder.Title, asmConfigurationBinder.InformationalVersion, asmConfigurationBinder.FrameworkName);

    var asmLoggerFactory = new AssemblyInfo(typeof(LoggerFactory).Assembly);
    logger.LogInformation("AssemblyTitle:{appTitle}; Version:{appVersion} Framework:{frameworkName}",
        asmLoggerFactory.Title, asmLoggerFactory.InformationalVersion, asmLoggerFactory.FrameworkName);

    logger.LogInformation("Service configure...");

    IServiceCollection services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(configuration);
    services.AddLogging();
    services.AddSerilog();
    services.AddScoped<Runner>();

    await using var rootServiceProvider = services.BuildServiceProvider();

    await using var scope = new AsyncDisposable<IServiceScope>(rootServiceProvider.CreateScope());

    var runner = scope.Service.ServiceProvider.GetRequiredService<Runner>();
    await runner.RunAsync().ConfigureAwait(false);
}
catch (Exception ex)
{
    AppLog.Logger.LogCritical(ex, "Application terminated unexpectedly");
}
finally
{
    Serilog.Log.CloseAndFlush();
}
