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
    void LogAssembly(AssemblyInfo assembly)
    {
        logger.LogInformation("AssemblyTitle:{title}; Version:{version} Framework:{framework}",
        assembly.Title, assembly.InformationalVersion, assembly.FrameworkName);
    }

    LogAssembly(new AssemblyInfo(typeof(ConfigurationBinder).Assembly));
    LogAssembly(new AssemblyInfo(typeof(LoggerFactory).Assembly));
    LogAssembly(new AssemblyInfo(typeof(SerilogApplicationBuilderExtensions).Assembly));

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
