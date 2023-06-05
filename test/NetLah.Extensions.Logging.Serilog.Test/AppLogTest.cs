using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Serilog;
using Xunit;

namespace NetLah.Extensions.Logging.Serilog.Test;

[Collection(LoggerFixtureCollection.CollectionName)]
public class AppLogTest
{
    private readonly LoggerFixture _fixture;

    public AppLogTest(LoggerFixture fixture)
    {
        _fixture = fixture;
    }

    private static IConfiguration GetConfig()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["key"] = "value" });
        var configuration = configurationBuilder.Build();
        return configuration;
    }

    [Fact]
    public void SilentLogger_Test()
    {
        var silentLogger = _fixture.SilentLogger;
        Assert.NotNull(silentLogger);
        Assert.Equal("SilentLogger", silentLogger.GetType().Name);
    }

    [Fact]
    public void NullLogger_Test()
    {
        var nullLogger = _fixture.NullLogger;
        Assert.NotNull(nullLogger);
        Assert.IsType<NullLogger>(nullLogger);
    }

    [Fact]
    public void InitLogger_Test()
    {
        _fixture.ResetLogger();

        AppLog.InitLogger("Test");
        var logger2 = AppLog.Logger;
        Assert.NotNull(logger2);

        AppLog.InitLogger("Test");
        var logger3 = AppLog.Logger;
        Assert.Same(logger3, logger2);
    }

    [Fact]
    public void InitLogger_Info_Test()
    {
        _fixture.ResetLogger();

        AppLog.InitLogger(global::Serilog.Events.LogEventLevel.Information, "Test");
        var logger2 = AppLog.Logger;
        Assert.NotNull(logger2);

        AppLog.InitLogger(global::Serilog.Events.LogEventLevel.Debug, "Test");
        var logger3 = AppLog.Logger;
        Assert.Same(logger3, logger2);
    }

    [Fact]
    public void CreateAppLogger_Type_Config()
    {
        var logger4a = AppLog.CreateAppLogger<AppLogTest>(GetConfig());
        var logger4 = AppLog.Logger;
        Assert.NotNull(logger4);
        Assert.NotSame(logger4, logger4a);
    }

    [Fact]
    public void CreateAppLogger_Config()
    {
        var logger4a = AppLog.CreateAppLogger(GetConfig());
        var logger4 = AppLog.Logger;
        Assert.NotNull(logger4);
        Assert.Same(logger4, logger4a);
    }

    [Fact]
    public void CreateAppLogger_Type_SetupLoggerConfiguration()
    {
        var mockSetupLoggerConfiguration = new Mock<Action<LoggerConfiguration>>();
        mockSetupLoggerConfiguration.Setup(invoke => invoke(It.IsAny<LoggerConfiguration>()))
            .Callback((LoggerConfiguration lc) => lc.ReadFrom.Configuration(GetConfig()).Enrich.FromLogContext())
            .Verifiable();
        var logger5a = AppLog.CreateAppLogger<AppLogTest>(mockSetupLoggerConfiguration.Object);
        var logger5 = AppLog.Logger;
        Assert.NotNull(logger5);
        Assert.NotSame(logger5, logger5a);
        mockSetupLoggerConfiguration.VerifyAll();
    }

    [Fact]
    public void CreateAppLogger_SetupLoggerConfiguration()
    {
        var mockSetupLoggerConfiguration = new Mock<Action<LoggerConfiguration>>();
        mockSetupLoggerConfiguration.Setup(invoke => invoke(It.IsAny<LoggerConfiguration>()))
            .Callback((LoggerConfiguration lc) => lc.ReadFrom.Configuration(GetConfig()).Enrich.FromLogContext())
            .Verifiable();
        var logger5a = AppLog.CreateAppLogger(mockSetupLoggerConfiguration.Object);
        var logger5 = AppLog.Logger;
        Assert.NotNull(logger5);
        Assert.Same(logger5, logger5a);
        mockSetupLoggerConfiguration.VerifyAll();
    }

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    [Fact]
    public void Init_Null_ConfigureLogger()
    {
        var result = Assert.Throws<ArgumentNullException>(() => AppLog.InitLogger((Action<LoggerConfiguration>)null));
        Assert.Equal("Value cannot be null. (Parameter 'configureLogger')", result.Message);
    }

    [Fact]
    public void Type_Null_Configuration()
    {
        var result = Assert.Throws<ArgumentNullException>(() => AppLog.CreateAppLogger<AppLogTest>((IConfiguration)null));
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", result.Message);
    }

    [Fact]
    public void Type_Null_ConfigureLogger()
    {
        var result = Assert.Throws<ArgumentNullException>(() => AppLog.CreateAppLogger<AppLogTest>((Action<LoggerConfiguration>)null));
        Assert.Equal("Value cannot be null. (Parameter 'configureLogger')", result.Message);
    }

    [Fact]
    public void Null_Configuration()
    {
        var result = Assert.Throws<ArgumentNullException>(() => AppLog.CreateAppLogger((IConfiguration)null));
        Assert.Equal("Value cannot be null. (Parameter 'configuration')", result.Message);
    }

    [Fact]
    public void Null_ConfigureLogger()
    {
        var result = Assert.Throws<ArgumentNullException>(() => AppLog.CreateAppLogger((Action<LoggerConfiguration>)null));
        Assert.Equal("Value cannot be null. (Parameter 'configureLogger')", result.Message);
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
}
