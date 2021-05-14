using Serilog;
using Serilog.Extensions.Logging;
using IFrameworkLogger = Microsoft.Extensions.Logging.ILogger;

namespace NetLah.Extensions.Logging
{
    internal class LazySerilogLoggerProvider
    {
        private readonly SerilogLoggerProvider _serilogLoggerProvider = new();
        private IFrameworkLogger _logger;
        private string _categoryName;
        private ILogger _serilogLogger;

        public IFrameworkLogger GetLogger(string categoryName)
        {
            var serilogLogger = Log.Logger;
            var name = categoryName ?? AppLog.CategoryName;
            if (_logger == null || _serilogLogger != serilogLogger || _categoryName != name)
            {
                _categoryName = name;
                _serilogLogger = serilogLogger;
                _logger = _serilogLoggerProvider.CreateLogger(name);
            }
            return _logger;
        }
    }
}
