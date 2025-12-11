using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using IFrameworkLogger = Microsoft.Extensions.Logging.ILogger;

namespace NetLah.Extensions.Logging;

#if NET8_0_OR_GREATER
public static class HostApplicationBuilderExtensions
{
    public static THostApplicationBuilder UseSerilog<THostApplicationBuilder>(this THostApplicationBuilder hostApplicationBuilder, Action<IFrameworkLogger>? applicationLoggerCreated = null)
        where THostApplicationBuilder : IHostApplicationBuilder
    {
        return hostApplicationBuilder == null
            ? throw new ArgumentNullException(nameof(hostApplicationBuilder))
            : hostApplicationBuilder.UseSerilog(builder => AppLog.CreateAppLogger(builder.Configuration),
            applicationLoggerCreated);
    }

    public static THostApplicationBuilder UseSerilog<THostApplicationBuilder>(this THostApplicationBuilder hostApplicationBuilder,
        Action<THostApplicationBuilder, LoggerConfiguration> loggerConfigurationFactory,
        Action<IFrameworkLogger>? applicationLoggerCreated = null)
        where THostApplicationBuilder : IHostApplicationBuilder
    {
        return hostApplicationBuilder == null
            ? throw new ArgumentNullException(nameof(hostApplicationBuilder))
            : loggerConfigurationFactory == null
            ? throw new ArgumentNullException(nameof(loggerConfigurationFactory))
            : hostApplicationBuilder
            .UseSerilog(builder => AppLog.CreateAppLogger(lc => loggerConfigurationFactory(builder, lc)),
                applicationLoggerCreated);
    }

    private static THostApplicationBuilder UseSerilog<THostApplicationBuilder>(this THostApplicationBuilder hostApplicationBuilder,
        Func<THostApplicationBuilder, IFrameworkLogger> configureLogger,
        Action<IFrameworkLogger>? applicationLoggerCreated = null)
        where THostApplicationBuilder : IHostApplicationBuilder
    {
        var logger = configureLogger(hostApplicationBuilder);
        applicationLoggerCreated?.Invoke(logger);
        hostApplicationBuilder.Logging
            .ClearProviders()
            .AddSerilog();
        return hostApplicationBuilder;
    }
}
#else
public static class HostApplicationBuilderExtensions
{
    public static HostApplicationBuilder UseSerilog(this HostApplicationBuilder hostApplicationBuilder, Action<IFrameworkLogger>? applicationLoggerCreated = null)
    {
        return hostApplicationBuilder == null
            ? throw new ArgumentNullException(nameof(hostApplicationBuilder))
            : hostApplicationBuilder.UseSerilog(builder => AppLog.CreateAppLogger(builder.Configuration),
            applicationLoggerCreated);
    }

    public static HostApplicationBuilder UseSerilog(this HostApplicationBuilder hostApplicationBuilder,
        Action<HostApplicationBuilder, LoggerConfiguration> loggerConfigurationFactory,
        Action<IFrameworkLogger>? applicationLoggerCreated = null)
    {
        return hostApplicationBuilder == null
            ? throw new ArgumentNullException(nameof(hostApplicationBuilder))
            : loggerConfigurationFactory == null
            ? throw new ArgumentNullException(nameof(loggerConfigurationFactory))
            : hostApplicationBuilder
            .UseSerilog(builder => AppLog.CreateAppLogger(lc => loggerConfigurationFactory(builder, lc)),
                applicationLoggerCreated);
    }

    private static HostApplicationBuilder UseSerilog(this HostApplicationBuilder hostApplicationBuilder,
        Func<HostApplicationBuilder, IFrameworkLogger> configureLogger,
        Action<IFrameworkLogger>? applicationLoggerCreated = null)
    {
        var logger = configureLogger(hostApplicationBuilder);
        applicationLoggerCreated?.Invoke(logger);
        hostApplicationBuilder.Logging
            .ClearProviders()
            .AddSerilog();
        return hostApplicationBuilder;
    }
}
#endif
