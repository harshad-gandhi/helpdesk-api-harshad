namespace HelpDesk.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
    (string Subject, string HtmlTemplate) GetTemplate(string templateType, string payloadJson);
}
