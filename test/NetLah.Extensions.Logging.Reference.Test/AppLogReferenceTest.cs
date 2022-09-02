namespace NetLah.Extensions.Logging.Reference.Test;

public class AppLogReferenceTest
{
    [Fact]
    public void NoReferenceTest()
    {
        var logger = AppLogReference.GetAppLogLogger();

        Assert.Null(logger);
    }
}