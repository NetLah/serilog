namespace SampleConsoleAppDependencyInjection;

internal class AsyncDisposable<TService> : IAsyncDisposable where TService : IDisposable
{
    public AsyncDisposable(TService service)
    {
        Service = service;
    }

    public TService Service { get; }

    public ValueTask DisposeAsync()
    {
        if (Service is IAsyncDisposable asyncDisposable)
        {
            return asyncDisposable.DisposeAsync();
        }

        Service.Dispose();
#if NETCOREAPP3_1
        return new ValueTask(Task.CompletedTask);
#else
        return ValueTask.CompletedTask;
#endif
    }
}
