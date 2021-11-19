using Microsoft.Extensions.Hosting;
using Serilog;
using IFrameworkLogger = Microsoft.Extensions.Logging.ILogger;

namespace NetLah.Extensions.Logging
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseSerilog2(this IHostBuilder hostBuilder, 
            Action<IFrameworkLogger>? applicationLoggerCreated = null)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            return hostBuilder.UseSerilog2(hostBuilderContext => AppLog.CreateAppLogger(hostBuilderContext.Configuration), 
                applicationLoggerCreated);
        }

        public static IHostBuilder UseSerilog2(this IHostBuilder hostBuilder, 
            Action<HostBuilderContext, LoggerConfiguration>? configureLogger, 
            Action<IFrameworkLogger>? applicationLoggerCreated = null)
        {
            if (hostBuilder == null)
                throw new ArgumentNullException(nameof(hostBuilder));

            if (configureLogger == null)
                throw new ArgumentNullException(nameof(configureLogger));

            return hostBuilder
                .UseSerilog2(hostBuilderContext => AppLog.CreateAppLogger(lc => configureLogger(hostBuilderContext, lc)), 
                    applicationLoggerCreated);
        }

        private static IHostBuilder UseSerilog2(this IHostBuilder hostBuilder, 
            Func<HostBuilderContext, IFrameworkLogger> configureLogger, 
            Action<IFrameworkLogger>? applicationLoggerCreated)
            => hostBuilder
                .ConfigureServices((hostBuilderContext, _) =>
                {
                    var logger = configureLogger(hostBuilderContext);
                    applicationLoggerCreated?.Invoke(logger);
                })
                .UseSerilog();
    }
}
