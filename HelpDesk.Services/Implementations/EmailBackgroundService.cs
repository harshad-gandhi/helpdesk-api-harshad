using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using HelpDesk.Services.Interfaces;
using HelpDesk.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using HelpDesk.Common.DTOs.ResultDTOs;

namespace HelpDesk.Services.Implementations;
    
/// <summary>
/// A background service that periodically processes pending emails from the outbox and sends them using the 
/// configured email service. It also updates the email status in the outbox repository.
/// </summary>
public class EmailBackgroundService(IConfiguration configuration, IServiceScopeFactory scopeFactory, ILogger<EmailBackgroundService> logger) : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<EmailBackgroundService> _logger = logger;

    /// <summary>
    /// Executes the background email sending loop.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token to stop the background service.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// Steps performed in the loop:
    /// <list type="bullet">
    ///   <item><description>Checks for pending emails in the outbox.</description></item>
    ///   <item><description>Sends each email and updates its status.</description></item>
    ///   <item><description>Logs errors encountered during sending.</description></item>
    ///   <item><description>Repeats periodically until <paramref name="stoppingToken"/> is signaled.</description></item>
    /// </list>
    /// </remarks>
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        double pollingInterval = _configuration.GetValue<double>("EmailBackgroundService:PollingIntervalSeconds", 15);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using IServiceScope? scope = _scopeFactory.CreateScope();
                IEmailOutboxRepository? emailOutboxRepo = scope.ServiceProvider.GetRequiredService<IEmailOutboxRepository>();
                IEmailService? emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                List<EmailOutboxResultDTO>? unsentEmails = await emailOutboxRepo.GetPendingEmailsAsync();

                foreach (EmailOutboxResultDTO? email in unsentEmails)
                {
                    try
                    {
                        (string subject, string htmlBody) = emailService.GetTemplate(email.TemplateType, email.Payload);
                        await emailService.SendEmailAsync(email.ToEmail, subject, htmlBody);
                        await emailOutboxRepo.UpdateEmailStatusAsync(email.Id, true, null);
                    }
                    catch (Exception ex)
                    {
                        await emailOutboxRepo.UpdateEmailStatusAsync(email.Id, false, ex.Message);
                        _logger.LogError(ex, "Error sending email to {Email}", email.ToEmail);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email background worker encountered an error.");
            }

            await Task.Delay(TimeSpan.FromSeconds(pollingInterval), stoppingToken);
        }
    }
}