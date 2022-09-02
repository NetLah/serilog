using Microsoft.Extensions.Logging;

namespace NetLah.Extensions.Logging;

public static class AppLogReference
{
    private static readonly Lazy<Func<string?, ILogger?>> _lazyDelegate = new(DelegateFactory);

    public static ILogger? GetAppLogLogger<TCategoryName>() => GetAppLogLogger(typeof(TCategoryName).FullName);

    public static ILogger? GetAppLogLogger(string? categoryName = "App")
    {
        try
        {
            return _lazyDelegate.Value(categoryName);
        }
        catch
        {
            // Do nothing
        }

        return default;
    }

    private static Func<string?, ILogger?> DelegateFactory()
    {
        try
        {
            var type = Type.GetType("NetLah.Extensions.Logging.AppLog, NetLah.Extensions.Logging.Serilog");
            if (type != null &&
                type.GetMethod("CreateLogger", new Type[] { typeof(string) }) is { } methodInfo &&
                typeof(ILogger).IsAssignableFrom(methodInfo.ReturnType))
            {
                return (Func<string?, ILogger?>)methodInfo.CreateDelegate(typeof(Func<string?, ILogger?>));
            }
        }
        catch
        {
            // Do nothing
        }

        return _ => null;
    }
}
