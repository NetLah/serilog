using Serilog;
using Xunit;
using FrameworkLogger = Microsoft.Extensions.Logging.ILogger;

namespace NetLah.Extensions.Logging.Serilog.Test;

public class LoggerFixture
{
    public ILogger SilentLogger { get; } = Log.Logger;

    public FrameworkLogger NullLogger { get; } = AppLog.Logger;

    public void ResetLogger()
    {
        Log.Logger = SilentLogger;
    }
}

[CollectionDefinition(LoggerFixtureCollection.CollectionName)]
public class LoggerFixtureCollection : ICollectionFixture<LoggerFixture>
{
    public const string CollectionName = "LoggerFixture collection";
}
