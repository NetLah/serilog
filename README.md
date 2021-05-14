# NetLah.Extensions.Logging.Serilog - .NET Library

[NetLah.Extensions.Logging.Serilog](https://www.nuget.org/packages/NetLah.Extensions.Logging.Serilog/) is a library which contains a set of reusable utility classes for wrapping Serilog to `Microsoft.Extensions.Logging.ILogger` and Serilog initialization as soon as the configuration available. The utility classes are `AppLog`, `AspNetCoreApplicationBuilderExtensions`, `HostBuilderExtensions`.

## Nuget package

[![NuGet](https://img.shields.io/nuget/v/NetLah.Extensions.Logging.Serilog.svg?style=flat-square&label=nuget&colorB=00b200)](https://www.nuget.org/packages/NetLah.Extensions.Logging.Serilog/)

## Getting started

### Reference

- ConsoleApp: https://github.com/serilog/serilog/wiki/Getting-Started#example-application

- WebApp: https://github.com/serilog/serilog-aspnetcore#serilogaspnetcore---

- Two-stage initialization: https://github.com/serilog/serilog-aspnetcore#two-stage-initialization

### ConsoleApp

```
using NetLah.Extensions.Logging;

public static void Main(string[] args)
{
    AppLog.InitLogger();
    try
    {
        AppLog.Logger.LogInformation("Application configure...");   // write log console only

        var configuration = ConfigurationBuilderBuilder.Create<Program>(args).Build();
        var logger = AppLog.CreateAppLogger<Program>(configuration);

        logger.LogInformation("Hello World!");      //  write log to sinks
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
```

### ConsoleApp with dependency injection

```
using NetLah.Extensions.Logging;

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
        return ValueTask.CompletedTask;
    }
}
```

### AspNetCore or Hosting application

```
using NetLah.Extensions.Logging;

public static void Main(string[] args)
{
    AppLog.InitLogger();
    try
    {
        AppLog.Logger.LogInformation("Application configure...");   // write log console only

        CreateHostBuilder(args).Build().Run();
    }
    catch (Exception ex)
    {
        AppLog.Logger.LogCritical(ex, "Host terminated unexpectedly");
    }
    finally
    {
        Serilog.Log.CloseAndFlush();
    }
}

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog2(logger => logger.LogInformation("Application initializing..."))    //  write log to sinks
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        AppLog.Logger.LogInformation("Startup constructor");    //  write log to sinks
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var logger = AppLog.Logger;
        logger.LogInformation("ConfigureServices...");          //  write log to sinks
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        var logger1 = AppLog.Logger;
        logger1.LogInformation("ConfigureApplication...");              //  write log to sinks
        logger.LogInformation("[Startup] ConfigureApplication...");     //  write log to sinks

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SampleWebApi v1"));
        }

        app.UseSerilogRequestLoggingLevel();

        app.UseHttpsRedirection();
    }
}
```
