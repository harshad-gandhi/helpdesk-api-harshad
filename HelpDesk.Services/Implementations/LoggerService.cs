using HelpDesk.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace HelpDesk.Services.Implementations
{
    public class LoggerService(ILogger<LoggerService> logger) : ILoggerService
    {
        private readonly ILogger _logger = logger;


        public void LogError(Exception exception, string message, params object[] args)
        {
            _logger.LogError(exception, message, args);
        }

    }
}