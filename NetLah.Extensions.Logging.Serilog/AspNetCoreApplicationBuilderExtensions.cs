using System;
using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Events;
using FrameworkLogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace NetLah.Extensions.Logging
{
    public static class AspNetCoreApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSerilogRequestLoggingLevel(this IApplicationBuilder applicationBuilder, FrameworkLogLevel logLevel = FrameworkLogLevel.Debug)
        {
            if (logLevel == FrameworkLogLevel.None)
                return applicationBuilder;

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

            return applicationBuilder.UseSerilogRequestLogging(opt => opt.GetLevel = (c, d, e) => d >= 500.0 || e != null ? LogEventLevel.Error : logEventLevel);
        }
    }
}
