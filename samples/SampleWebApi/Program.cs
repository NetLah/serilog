using NetLah.Diagnostics;
using NetLah.Extensions.Logging;
using Serilog;
using Serilog.Events;

AppLog.InitLogger(LogEventLevel.Information);

AppLog.Logger.LogInformation("Application starting...");

try
{
    var appInfo = ApplicationInfo.TryInitialize(null);
    var builder = WebApplication.CreateBuilder(args);

    builder.UseSerilog(logger => logger.LogInformation("Application initializing... AppTitle:{appTitle}; Version:{appVersion} BuildTime:{appBuildTime}; Framework:{frameworkName}",
        appInfo.Title, appInfo.InformationalVersion, appInfo.BuildTimestampLocal, appInfo.FrameworkName));

    var logger = AppLog.Logger;
    var asmConfigurationBinder = new AssemblyInfo(typeof(ConfigurationBinder).Assembly);
    logger.LogInformation("AssemblyTitle:{appTitle}; Version:{appVersion} Framework:{frameworkName}",
        asmConfigurationBinder.Title, asmConfigurationBinder.InformationalVersion, asmConfigurationBinder.FrameworkName);

    var asmLoggerFactory = new AssemblyInfo(typeof(LoggerFactory).Assembly);
    logger.LogInformation("AssemblyTitle:{appTitle}; Version:{appVersion} Framework:{frameworkName}",
        asmLoggerFactory.Title, asmLoggerFactory.InformationalVersion, asmLoggerFactory.FrameworkName);

    var asmSerilogAspNetCore = new AssemblyInfo(typeof(SerilogApplicationBuilderExtensions).Assembly);
    logger.LogInformation("AssemblyTitle:{title}; Version:{version} Framework:{framework}",
        asmSerilogAspNetCore.Title, asmSerilogAspNetCore.InformationalVersion, asmSerilogAspNetCore.FrameworkName);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLoggingLevel();

    app.UseHttpsRedirection();

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
