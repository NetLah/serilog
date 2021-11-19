using Microsoft.Extensions.Logging;

namespace SampleConsoleAppDependencyInjection;

internal class Runner : IAsyncDisposable
{
    private readonly ILogger _logger;

    public Runner(ILogger<Runner> logger)
    {
        _logger = logger;
    }

    public ValueTask DisposeAsync()
    {
        _logger.LogInformation("[DisposeAsync] Runner");        //  write log to sinks
#if NETCOREAPP3_1
            return new ValueTask(Task.CompletedTask);
#else
        return ValueTask.CompletedTask;
#endif
    }

    public async Task RunAsync()
    {
        await Task.CompletedTask;

        _logger.LogInformation("[RunAsync] Hello World!");      //  write log to sinks
    }
}
