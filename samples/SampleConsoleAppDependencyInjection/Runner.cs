using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SampleConsoleAppDependencyInjection
{
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

            return ValueTask.CompletedTask;
        }

        public async Task RunAsync()
        {
            await Task.CompletedTask;

            _logger.LogInformation("[RunAsync] Hello World!");      //  write log to sinks
        }
    }
}
