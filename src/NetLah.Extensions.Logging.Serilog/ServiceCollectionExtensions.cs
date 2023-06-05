using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using ISerilogLogger = Serilog.ILogger;

namespace NetLah.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSerilog(this IServiceCollection services, ISerilogLogger? logger = null, bool dispose = false)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(services);
#else
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }
#endif

        var previous = services.FirstOrDefault(s => s.ServiceType == typeof(ILoggerFactory));
        if (previous != null && previous != services)
        {
            services.Remove(previous);
        }

        services.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(logger, dispose));

        if (logger != null)
        {
            // This won't (and shouldn't) take ownership of the logger. 
            services.AddSingleton(logger);
        }

        return services;
    }
}
