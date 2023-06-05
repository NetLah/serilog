using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using IFrameworkLogger = Microsoft.Extensions.Logging.ILogger;

namespace NetLah.Extensions.Logging;

public static class AppLog
{
    private static Func<string?, IFrameworkLogger> _loggerFactory = _ => NullLogger.Instance;
    private static LazySerilogLoggerProvider? _lazyLogger;
    private static string? _categoryName;

    public static IFrameworkLogger Logger => _loggerFactory(null);

    public static IFrameworkLogger CreateLogger(string? categoryName)
    {
        return _loggerFactory(categoryName);
    }

    public static IFrameworkLogger CreateLogger<TCategoryName>()
    {
        return CreateLogger(GetCategoryName<TCategoryName>());
    }

    public static string CategoryName => _categoryName ?? "App";

    public static void InitLogger(string? categoryName = null)
    {
        InitLogger(lc => lc.MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            , categoryName);
    }

    public static void InitLogger(Action<LoggerConfiguration> configureLogger, string? categoryName = null)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(configureLogger);
#else
        if (configureLogger == null)
        {
            throw new ArgumentNullException(nameof(configureLogger));
        }
#endif

        if (string.IsNullOrWhiteSpace(categoryName))
        {
            categoryName = null;
        }

        _categoryName = categoryName;

        if (Log.Logger.GetType().Name == "SilentLogger")
        {
            var loggerConfiguration = new LoggerConfiguration();
            configureLogger(loggerConfiguration);
            Log.Logger = loggerConfiguration.CreateLogger();
        }

        SetupLazyLogger();
    }

    public static IFrameworkLogger CreateAppLogger<TCategoryName>(IConfiguration configuration)
    {
        return CreateAppLogger(configuration, GetCategoryName<TCategoryName>());
    }

    public static IFrameworkLogger CreateAppLogger<TCategoryName>(Action<LoggerConfiguration> configureLogger)
    {
        return CreateAppLogger(configureLogger, GetCategoryName<TCategoryName>());
    }

    public static IFrameworkLogger CreateAppLogger(IConfiguration configuration, string? categoryName = null)
    {
        return configuration == null
            ? throw new ArgumentNullException(nameof(configuration))
            : CreateAppLogger(lc => lc.ReadFrom.Configuration(configuration).Enrich.FromLogContext(), categoryName);
    }

    public static IFrameworkLogger CreateAppLogger(Action<LoggerConfiguration> configureLogger, string? categoryName = null)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(configureLogger);
#else
        if (configureLogger == null)
        {
            throw new ArgumentNullException(nameof(configureLogger));
        }
#endif

        var loggerConfiguration = new LoggerConfiguration();
        configureLogger(loggerConfiguration);

        Log.CloseAndFlush();
        Log.Logger = loggerConfiguration.CreateLogger();

        SetupLazyLogger();
        var logger = _loggerFactory(categoryName);
        logger.LogDebug("Logger has been initialized.");
        return logger;
    }

    private static string? GetCategoryName<TCategoryName>()
    {
        return typeof(TCategoryName).FullName;
    }

    private static void SetupLazyLogger()
    {
        _lazyLogger ??= new LazySerilogLoggerProvider();
        _loggerFactory = _lazyLogger.GetLogger;
    }
}
