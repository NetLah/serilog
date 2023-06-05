using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetLah.Diagnostics;
using NetLah.Extensions.Configuration;
using NetLah.Extensions.Logging;
using SampleConsoleAppDependencyInjection;

AppLog.InitLogger();

try
{
    var appInfo = ApplicationInfo.TryInitialize(null);
    AppLog.Logger.LogInformation("Application configure...");   // write log console only

    var configuration = ConfigurationBuilderBuilder.Create<Program>(args).Build();
    var logger = AppLog.CreateAppLogger<Program>(configuration);
    logger.LogInformation("Application initializing... AppTitle:{appTitle}; Version:{appVersion} BuildTime:{appBuildTime}; Framework:{frameworkName}",
        appInfo.Title, appInfo.InformationalVersion, appInfo.BuildTimestampLocal, appInfo.FrameworkName);
    logger.LogInformation("Service configure...");      // write log to sinks

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
