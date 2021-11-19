using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Extensions.Hosting;
using IFrameworkLogger = Microsoft.Extensions.Logging.ILogger;

namespace NetLah.Extensions.Logging
{
#if NET6_0_OR_GREATER
    public static class AspNetCoreWebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder webApplicationBuilder,
            Action<IFrameworkLogger>? applicationLoggerCreated = null)
        {
            if (webApplicationBuilder == null)
                throw new ArgumentNullException(nameof(webApplicationBuilder));

            return webApplicationBuilder.UseSerilog(builder => AppLog.CreateAppLogger(builder.Configuration), 
                applicationLoggerCreated);
        }

        public static WebApplicationBuilder UseSerilog(this WebApplicationBuilder webApplicationBuilder,
            Action<WebApplicationBuilder, LoggerConfiguration> loggerConfigurationFactory,
            Action<IFrameworkLogger>? applicationLoggerCreated = null)
        {
            if (webApplicationBuilder == null)
                throw new ArgumentNullException(nameof(webApplicationBuilder));

            if (loggerConfigurationFactory == null)
                throw new ArgumentNullException(nameof(loggerConfigurationFactory));

            return webApplicationBuilder
                .UseSerilog(builder => AppLog.CreateAppLogger(lc => loggerConfigurationFactory(builder, lc)), 
                    applicationLoggerCreated);
        }

        private static WebApplicationBuilder UseSerilog(this WebApplicationBuilder webApplicationBuilder,
            Func<WebApplicationBuilder, IFrameworkLogger> configureLogger,
            Action<IFrameworkLogger>? applicationLoggerCreated = null)
        {
            var logger = configureLogger(webApplicationBuilder);

            webApplicationBuilder.Services.AddSerilog();

            // Registered to provide two services...
            var diagnosticContext = new DiagnosticContext(Log.Logger);
            webApplicationBuilder.Services.AddSingleton(diagnosticContext);
            webApplicationBuilder.Services.AddSingleton<IDiagnosticContext>(diagnosticContext);

            applicationLoggerCreated?.Invoke(logger);

            return webApplicationBuilder;
        }
    }
#endif
}
