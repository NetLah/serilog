using NetLah.Diagnostics;
using NetLah.Extensions.Logging;

namespace SampleWebApi;

internal class Program
{
    public static void Main(string[] args)
    {
        ApplicationInfo.Initialize(null);
        AppLog.InitLogger();
        try
        {
            AppLog.Logger.LogInformation("Application configure...");   // write log console only

            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            AppLog.Logger.LogCritical(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Serilog.Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog2(logger =>
            {
                if (ApplicationInfo.Instance is { } appInfo)
                {
                    logger.LogInformation("Application initializing... AppTitle:{appTitle}; Version:{appVersion} BuildTime:{appBuildTime}; Framework:{frameworkName}",
                        appInfo.Title, appInfo.InformationalVersion, appInfo.BuildTimestampLocal, appInfo.FrameworkName);
                }
            })    //  write log to sinks
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
