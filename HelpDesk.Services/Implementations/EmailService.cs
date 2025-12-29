using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using HelpDesk.Common.Constants;
using HelpDesk.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace HelpDesk.Services.Implementations;

/// <summary>
/// Provides email-related services such as sending emails and generating email templates
/// for various scenarios like verification, password reset, and project invitations.
/// </summary>
public class EmailService(IConfiguration configuration, IWebHostEnvironment env) : IEmailService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IWebHostEnvironment _env = env;

    /// <summary>
    /// Sends an email to the specified recipient with the given subject and HTML body.
    /// </summary>
    /// <param name="to">The recipient email address.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="htmlBody">The HTML content of the email.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        MimeMessage? email = new();
        email.From.Add(MailboxAddress.Parse(_configuration["Email:From"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

        using SmtpClient? smtp = new();
        await smtp.ConnectAsync(_configuration["Email:SmtpHost"], int.Parse(_configuration["Email:Port"]!), SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    /// <summary>
    /// Generates an email template (subject and HTML) based on the template type and payload.
    /// </summary>
    /// <param name="templateType">The type of the email template (e.g., "verifyemail", "passwordreset", "projectinvitation").</param>
    /// <param name="payloadJson">A JSON string containing key-value pairs to populate the template.</param>
    /// <returns>A tuple containing <c>Subject</c> and <c>HtmlTemplate</c>.</returns>
    public (string Subject, string HtmlTemplate) GetTemplate(string templateType, string payloadJson)
    {
        Dictionary<string, string>? payload = JsonSerializer.Deserialize<Dictionary<string, string>>(payloadJson)!;

        string appLogo = ImagePathBase64.APP_LOGO;

        string subject;
        string html = string.Empty;
        string templateFolder = Path.Combine(_env.WebRootPath, "email-templates");
        string templateFile = Path.Combine(templateFolder, $"{templateType.ToLower()}.html");

        if (!File.Exists(templateFile))
        {
            subject = "Default Subject";
            html = "<p>Template not found.</p>";
            return (subject, html);
        }

        string templateContent = File.ReadAllText(templateFile);
        string actionLink = string.Empty;

        switch (templateType.ToLower())
        {
            case "verifyemail":
                subject = "Verify your email address";
                actionLink = $"{_configuration["App:FrontendUrl"]}/verify-email?token={payload["VerificationToken"]}";
                break;

            case "passwordreset":
                subject = "Password Reset Request";
                actionLink = $"{_configuration["App:FrontendUrl"]}/reset-password?token={payload["ResetToken"]}";
                break;

            case "invitation":
                subject = "You're Invited to HelpDesk!";
                actionLink = $"{_configuration["App:FrontendUrl"]}/invitation?token={payload["InvitationToken"]}";
                break;

            default:
                subject = templateType.ToLower();
                html = "<p>Default content</p>";
                return (subject, html);
        }

        // Replace placeholders in template
        html = templateContent
            .Replace("{{ActionLink}}", actionLink)
            .Replace("{{APP_LOGO}}", appLogo);

        return (subject, html);
    }

}
