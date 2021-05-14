using System;
using Microsoft.Extensions.Hosting;
using Moq;
using Serilog;
using Xunit;
using FrameworkLogger = Microsoft.Extensions.Logging.ILogger;

namespace NetLah.Extensions.Logging.Serilog.Test
{
    [Collection(LoggerFixtureCollection.CollectionName)]
    public class HostBuilderExtensionsTest
    {
        public HostBuilderExtensionsTest(LoggerFixture fixture) { }

        [Fact]
        public void UseSerilog2_ActionFrameworkLogger_NullHostBuilder()
        {
            var result = Assert.Throws<ArgumentNullException>(() => HostBuilderExtensions.UseSerilog2(null, null));
            Assert.Equal("Value cannot be null. (Parameter 'hostBuilder')", result.Message);
        }

        [Fact]
        public void UseSerilog2_ConfigureLogger_NullHostBuilder()
        {
            var result = Assert.Throws<ArgumentNullException>(() => HostBuilderExtensions.UseSerilog2(null, (c, lc) => { }, null));
            Assert.Equal("Value cannot be null. (Parameter 'hostBuilder')", result.Message);
        }

        [Fact]
        public void UseSerilog2_ConfigureLogger_NullConfigureLogger()
        {
            var result = Assert.Throws<ArgumentNullException>(() => HostBuilderExtensions.UseSerilog2(new HostBuilder(), null, null));
            Assert.Equal("Value cannot be null. (Parameter 'configureLogger')", result.Message);
        }

        [Fact]
        public void UseSerilog2_Configuration()
        {
            var mock = new Mock<Action<FrameworkLogger>>();
            mock.Setup(invoke => invoke(It.IsAny<FrameworkLogger>())).Verifiable();
            IHostBuilder hostBuilder = new HostBuilder();
            HostBuilderExtensions.UseSerilog2(hostBuilder, mock.Object);
            hostBuilder.Build();
            mock.VerifyAll();
        }

        [Fact]
        public void UseSerilog2_LoggerConfiguration()
        {
            var mock = new Mock<Action<FrameworkLogger>>();
            IHostBuilder hostBuilder = new HostBuilder();
            var mockSetupLoggerConfiguration = new Mock<Action<HostBuilderContext, LoggerConfiguration>>();
            mockSetupLoggerConfiguration.Setup(invoke => invoke(It.IsAny<HostBuilderContext>(), It.IsAny<LoggerConfiguration>()))
                .Callback((HostBuilderContext c, LoggerConfiguration lc) => lc.ReadFrom.Configuration(c.Configuration).Enrich.FromLogContext())
                .Verifiable();
            HostBuilderExtensions.UseSerilog2(hostBuilder, mockSetupLoggerConfiguration.Object, mock.Object);
            hostBuilder.Build();
            mockSetupLoggerConfiguration.VerifyAll();
            mock.Verify(invoke => invoke(It.IsAny<FrameworkLogger>()), Times.Once);
        }
    }
}
