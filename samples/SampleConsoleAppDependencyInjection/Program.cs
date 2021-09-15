using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetLah.Extensions.Configuration;
using NetLah.Extensions.Logging;

namespace SampleConsoleAppDependencyInjection
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            AppLog.InitLogger();
            try
            {
                AppLog.Logger.LogInformation("Application configure...");   // write log console only

                var configuration = ConfigurationBuilderBuilder.Create<Program>(args).Build();
                var logger = AppLog.CreateAppLogger<Program>(configuration);
                logger.LogInformation("Service configure...");      // write log to sinks

                IServiceCollection services = new ServiceCollection();
                services.AddSingleton<IConfiguration>(configuration);
                services.AddLogging();
                services.AddSerilog();
                services.AddScoped<Runner>();

                await using var rootServiceProvider = services.BuildServiceProvider();

                await using var scope = new AsyncDisposable<IServiceScope>(rootServiceProvider.CreateScope());

                var runner = scope.Service.ServiceProvider.GetRequiredService<Runner>();
                await runner.RunAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                AppLog.Logger.LogCritical(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }
        }

        private class AsyncDisposable<TService> : IAsyncDisposable where TService : IDisposable
        {
            public AsyncDisposable(TService service) => this.Service = service;

            public TService Service { get; }

            public ValueTask DisposeAsync()
            {
                if (Service is IAsyncDisposable asyncDisposable)
                    return asyncDisposable.DisposeAsync();

                Service.Dispose();
#if NETCOREAPP3_1
                return new ValueTask(Task.CompletedTask);
#else
                return ValueTask.CompletedTask;
#endif
            }
        }
    }
}
