using Microsoft.Extensions.Logging;
using NetLah.Extensions.Configuration;
using NetLah.Extensions.Logging;

AppLog.InitLogger();
try
{
    AppLog.Logger.LogInformation("Application configure...");   // write log console only

    var configuration = ConfigurationBuilderBuilder.Create<Program>(args).Build();
    var logger = AppLog.CreateAppLogger<Program>(configuration);

    logger.LogInformation("Hello World!");      //  write log to sinks
}
catch (Exception ex)
{
    AppLog.Logger.LogCritical(ex, "Application terminated unexpectedly");
}
finally
{
    Serilog.Log.CloseAndFlush();
}
