using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;
using FrameworkLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace NetLah.Extensions.Logging;

public static class AspNetCoreApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSerilogRequestLoggingLevel(this IApplicationBuilder applicationBuilder, FrameworkLogLevel logLevel = FrameworkLogLevel.Debug)
    {
        if (logLevel == FrameworkLogLevel.None)
        {
            return applicationBuilder;
        }

        var logEventLevel = logLevel switch
        {
            FrameworkLogLevel.Trace => LogEventLevel.Verbose,
            FrameworkLogLevel.Debug => LogEventLevel.Debug,
            FrameworkLogLevel.Information => LogEventLevel.Information,
            FrameworkLogLevel.Warning => LogEventLevel.Warning,
            FrameworkLogLevel.Error => LogEventLevel.Error,
            FrameworkLogLevel.Critical => LogEventLevel.Fatal,
            _ => throw new NotSupportedException("Loglevel " + logLevel),
        };

#pragma warning disable S3358 // Ternary operators should not be nested
        return applicationBuilder.UseSerilogRequestLogging(
            opt => opt.GetLevel =
                (c, d, e) =>
                    e != null || c.Response.StatusCode >= 500
                    ? LogEventLevel.Error
                    : (d >= 500.0 ? LogEventLevel.Warning : logEventLevel));
#pragma warning restore S3358 // Ternary operators should not be nested
    }
}
