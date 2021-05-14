using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace NetLah.Extensions.Logging.Serilog.Test
{
    [Collection(LoggerFixtureCollection.CollectionName)]
    public class ServiceCollectionExtensionsTest
    {
        public ServiceCollectionExtensionsTest(LoggerFixture fixture) { }

        [Fact]
        public void AddSerilog_NullServices()
        {
            var result = Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddSerilog(services: null));
            Assert.Equal("Value cannot be null. (Parameter 'services')", result.Message);
        }

        [Fact]
        public void AddSerilog_Test()
        {
            var type = typeof(DefaultServiceProviderFactory);
            var services = new ServiceCollection();
            services.AddLogging();
            var s1 = Assert.Single(services, s => s.ServiceType == typeof(ILoggerFactory));
            ServiceCollectionExtensions.AddSerilog(services, logger: null, dispose: false);
            var s2 = Assert.Single(services, s => s.ServiceType == typeof(ILoggerFactory));
            Assert.NotSame(s2, s1);
        }
    }
}
