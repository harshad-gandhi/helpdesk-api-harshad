namespace HelpDesk.Services.Interfaces
{
    public interface ILoggerService
    {
        void LogError(Exception exception, string message, params object[] args);
    }
}