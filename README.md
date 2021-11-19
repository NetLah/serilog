# NetLah.Extensions.Logging.Serilog - .NET Library

[NetLah.Extensions.Logging.Serilog](https://www.nuget.org/packages/NetLah.Extensions.Logging.Serilog/) and [NetLah.Extensions.Logging.Serilog.AspNetCore](https://www.nuget.org/packages/NetLah.Extensions.Logging.Serilog.AspNetCore/) are a library which contains a set of reusable utility classes for initializing Serilog and wrapping Serilog to `Microsoft.Extensions.Logging.ILogger` for ASP.NETCore and ConsoleApp. The utility classes are `AppLog`, `AspNetCoreApplicationBuilderExtensions`, `HostBuilderExtensions`.

## Nuget package

[![NuGet](https://img.shields.io/nuget/v/NetLah.Extensions.Logging.Serilog.svg?style=flat-square&label=nuget&colorB=00b200)](https://www.nuget.org/packages/NetLah.Extensions.Logging.Serilog/)
[![NuGet](https://img.shields.io/nuget/v/NetLah.Extensions.Logging.Serilog.AspNetCore.svg?style=flat-square&label=nuget&colorB=00b200)](https://www.nuget.org/packages/NetLah.Extensions.Logging.Serilog.AspNetCore/)

## Build Status

[![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2FNetLah%2Fserilog%2Fbadge%3Fref%3Dmain&style=flat)](https://actions-badge.atrox.dev/NetLah/serilog/goto?ref=main)

## Getting started

### Reference

- ConsoleApp: https://github.com/serilog/serilog/wiki/Getting-Started#example-application

- WebApp: https://github.com/serilog/serilog-aspnetcore#serilogaspnetcore---

- Two-stage initialization: https://github.com/serilog/serilog-aspnetcore#two-stage-initialization

### Sample appsettings.json

```json
{
  "AllowedHosts": "*",
  "Serilog": {
    "Using": ["Serilog.Sinks.Console"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "App": "Information",
        "System": "Warning",
        "Microsoft": "Warning",
        "Microsoft.AspNetCore.Authentication": "Information",
        "Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker": "Error",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "WriteTo:0": { "Name": "Console" },
    "Enrich": ["FromLogContext", "WithMachineName"],
    "Destructure": [
      {
        "Name": "ToMaximumDepth",
        "Args": { "maximumDestructuringDepth": 4 }
      },
      {
        "Name": "ToMaximumStringLength",
        "Args": { "maximumStringLength": 100 }
      },
      {
        "Name": "ToMaximumCollectionCount",
        "Args": { "maximumCollectionCount": 10 }
      }
    ],
    "Properties": {}
  }
}
```

### Sample appsettings.Development.json

```json
{
  "Serilog": {
    "Using:1": "Serilog.Sinks.Debug",
    "Using:2": "Serilog.Sinks.File",
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "App": "Debug"
      }
    },
    "WriteTo:1": { "Name": "Debug" },
    "WriteTo:2": {
      "Name": "File",
      "Args": {
        "path": "Logs/sample-.log",
        "rollingInterval": "Day"
      }
    }
    //"WriteTo:3": {
    //  "Name": "Seq",
    //  "Args": {
    //    "serverUrl": "https://seq",
    //    "apiKey": "ZrV42..."
    //  }
    //},
  }
}
```

### ASP.NETCore 6.0

```csharp
using NetLah.Extensions.Logging;

AppLog.InitLogger();
AppLog.Logger.LogInformation("Application starting...");
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.UseSerilog(logger => logger.LogInformation("Application initializing..."));

    // Add services to the container.

    builder.Services.AddControllers();

    var app = builder.Build();

    // Configure the HTTP request pipeline.

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    AppLog.Logger.LogCritical(ex, "Application terminated unexpectedly");
}
finally
{
    Serilog.Log.CloseAndFlush();
}
```

### ConsoleApp

```csharp
using Microsoft.Extensions.Logging;
using NetLah.Extensions.Configuration;
using NetLah.Extensions.Logging;

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
```

### ConsoleApp with Dependency Injection

```csharp
using Microsoft.Extensions.Logging;
using NetLah.Extensions.Configuration;
using NetLah.Extensions.Logging;

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

internal class AsyncDisposable<TService> : IAsyncDisposable where TService : IDisposable
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
```

### AspNetCore or Hosting application

```csharp
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
