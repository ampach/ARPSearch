using System;
using log4net;
using Sitecore.Diagnostics;

namespace ARPSearch.Logging
{
    public class LogService : ILogService
    {
        private readonly ILog _logger;

        public LogService()
        {
            _logger = LoggerFactory.GetLogger("Sitecore.Logging.ARPSearch");
        }

        public void Debug(string message)
        {
            Assert.ArgumentNotNull(message, "message");
            _logger.Debug(message);
        }

        public void Debug(string message, Exception exception)
        {
            Assert.ArgumentNotNull(message, "message");
            Assert.ArgumentNotNull(exception, "exception");

            _logger.Debug(message, exception);
        }

        public void Info(string message)
        {
            Assert.ArgumentNotNull(message, "message");

            _logger.Info(message);
        }

        public void Info(string message, Exception exception)
        {
            Assert.ArgumentNotNull(message, "message");
            Assert.ArgumentNotNull(exception, "exception");

            _logger.Info(message, exception);
        }

        public void Warn(string message)
        {
            Assert.ArgumentNotNull(message, "message");

            _logger.Warn(message);
        }

        public void Warn(string message, Exception exception)
        {
            Assert.ArgumentNotNull(message, "message");
            Assert.ArgumentNotNull(exception, "exception");

            _logger.Warn(message, exception);
        }

        public void Error(string message)
        {
            Assert.ArgumentNotNull(message, "message");

            _logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            Assert.ArgumentNotNull(message, "message");
            Assert.ArgumentNotNull(exception, "exception");

            _logger.Error(message, exception);
        }

        public void Fatal(string message)
        {
            Assert.ArgumentNotNull(message, "message");

            _logger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            Assert.ArgumentNotNull(message, "message");
            Assert.ArgumentNotNull(exception, "exception");

            _logger.Fatal(message, exception);
        }
    }
}