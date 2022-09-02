using Xunit;

namespace NetLah.Extensions.Logging.Serilog.Test;

public class AppLogReferenceTest
{
    [Fact]
    public void HasAppLogReferenceTest()
    {
        var logger = AppLogReference.GetAppLogLogger<AppLogReferenceTest>();

        Assert.NotNull(logger);
    }
}
