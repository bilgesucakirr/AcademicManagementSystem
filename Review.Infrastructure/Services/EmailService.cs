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
        var subject = $"Review Invitation: {submissionTitle}";
        var body = $@"
            <div style='font-family: sans-serif; border: 1px solid #ddd; padding: 20px;'>
                <h2>Jarvis Academic Management System</h2>
                <p>You have been invited to review the following manuscript:</p>
                <p><strong>Title:</strong> {submissionTitle}</p>
                <p>Please click the button below to respond to this invitation:</p>
                <a href='{link}' style='background: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>View Invitation</a>
            </div>";

        await SendEmailAsync(toEmail, subject, body);
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