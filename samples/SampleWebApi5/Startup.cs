using Microsoft.OpenApi.Models;
using NetLah.Diagnostics;
using NetLah.Extensions.Logging;
using Serilog;

namespace SampleWebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        AppLog.Logger.LogInformation("Startup constructor");    //  write log to sinks
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        var logger = AppLog.Logger;
        logger.LogInformation("ConfigureServices...");          //  write log to sinks

        var asmSerilogAspNetCore = new AssemblyInfo(typeof(SerilogApplicationBuilderExtensions).Assembly);
        logger.LogInformation("AssemblyTitle:{title}; Version:{version} Framework:{framework}",
            asmSerilogAspNetCore.Title, asmSerilogAspNetCore.InformationalVersion, asmSerilogAspNetCore.FrameworkName);

        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "SampleWebApi", Version = "v1" });
        });
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

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
