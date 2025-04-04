using Microsoft.AspNetCore.Builder;
using Serilog;
using IFrameworkLogger = Microsoft.Extensions.Logging.ILogger;

namespace NetLah.Extensions.Logging;

#if NET6_0_OR_GREATER
public static class AspNetCoreWebApplicationBuilderExtensions
{
    public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder webApplicationBuilder,
        Action<IFrameworkLogger>? applicationLoggerCreated = null)
    {
        return webApplicationBuilder == null
            ? throw new ArgumentNullException(nameof(webApplicationBuilder))
            : webApplicationBuilder.UseSerilog(builder => AppLog.CreateAppLogger(builder.Configuration),
            applicationLoggerCreated);
    }

    public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder webApplicationBuilder,
        Action<WebApplicationBuilder, LoggerConfiguration> loggerConfigurationFactory,
        Action<IFrameworkLogger>? applicationLoggerCreated = null)
    {
        return webApplicationBuilder == null
            ? throw new ArgumentNullException(nameof(webApplicationBuilder))
            : loggerConfigurationFactory == null
            ? throw new ArgumentNullException(nameof(loggerConfigurationFactory))
            : webApplicationBuilder
            .UseSerilog(builder => AppLog.CreateAppLogger(lc => loggerConfigurationFactory(builder, lc)),
                applicationLoggerCreated);
    }

    private static WebApplicationBuilder UseSerilog(this WebApplicationBuilder webApplicationBuilder,
        Func<WebApplicationBuilder, IFrameworkLogger> configureLogger,
        Action<IFrameworkLogger>? applicationLoggerCreated = null)
    {
        var logger = configureLogger(webApplicationBuilder);
        applicationLoggerCreated?.Invoke(logger);
        webApplicationBuilder.Host.UseSerilog();
        return webApplicationBuilder;
    }
}
#endif
