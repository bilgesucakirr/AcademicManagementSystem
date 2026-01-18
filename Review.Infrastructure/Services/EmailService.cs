using Microsoft.Extensions.Configuration;
using Review.Application.Contracts;
using System.Net;
using System.Net.Mail;

namespace Review.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendInvitationEmailAsync(string toEmail, string assignmentId, string submissionTitle)
    {
        var link = $"https://localhost:7018/review-response/{assignmentId}";
        var body = GetHtmlTemplate("Review Invitation", $@"
            <p>You have been invited to review the following manuscript:</p>
            <p style='background: #f1f5f9; padding: 15px; border-radius: 8px; font-weight: bold; color: #0f172a;'>{submissionTitle}</p>
            <p>Your expertise is essential for maintaining the quality of our publications. Please respond to this invitation at your earliest convenience.</p>", link, "Respond to Invitation");

        await SendEmailAsync(toEmail, $"Review Invitation: {submissionTitle}", body);
    }

    public async Task SendRevisionNotificationAsync(string toEmail, string submissionTitle)
    {
        var body = GetHtmlTemplate("Revision Uploaded", $@"
            <p>The author has uploaded a revised version of the following manuscript:</p>
            <p style='background: #f1f5f9; padding: 15px; border-radius: 8px; font-weight: bold; color: #0f172a;'>{submissionTitle}</p>
            <p>Please log in to your dashboard to conduct the next round of review.</p>", "https://localhost:7018/my-reviews", "View My Reviews");

        await SendEmailAsync(toEmail, $"New Revision: {submissionTitle}", body);
    }

    private string GetHtmlTemplate(string title, string content, string actionUrl, string actionText)
    {
        return $@"
        <div style='font-family: Segoe UI, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden;'>
            <div style='background: #0f172a; padding: 25px; text-align: center; color: white;'>
                <h2 style='margin: 0;'>JARVIS</h2>
                <div style='font-size: 11px; opacity: 0.7; text-transform: uppercase;'>Reviewer Portal</div>
            </div>
            <div style='padding: 40px; color: #334155;'>
                <h3 style='margin-top: 0;'>{title}</h3>
                {content}
                <div style='margin-top: 35px; text-align: center;'>
                    <a href='{actionUrl}' style='background: #3b82f6; color: white; padding: 14px 25px; text-decoration: none; border-radius: 8px; font-weight: bold;'>{actionText}</a>
                </div>
            </div>
        </div>";
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpClient = new SmtpClient(_config["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(_config["EmailSettings:Port"]!),
            Credentials = new NetworkCredential(_config["EmailSettings:SenderEmail"], _config["EmailSettings:Password"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config["EmailSettings:SenderEmail"]!, _config["EmailSettings:SenderName"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(toEmail);
        await smtpClient.SendMailAsync(mailMessage);
    }
}