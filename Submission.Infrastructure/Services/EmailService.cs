using Microsoft.Extensions.Configuration;
using Submission.Application.Contracts;
using System.Net;
using System.Net.Mail;

namespace Submission.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) => _config = config;

    public async Task SendSubmissionReceiptAsync(string toEmail, string authorName, string title)
    {
        var body = $"<h3>Dear {authorName},</h3><p>Your submission '{title}' has been successfully received.</p>";

        var smtpClient = new SmtpClient(_config["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(_config["EmailSettings:Port"]!),
            Credentials = new NetworkCredential(_config["EmailSettings:SenderEmail"], _config["EmailSettings:Password"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config["EmailSettings:SenderEmail"]!, _config["EmailSettings:SenderName"]),
            Subject = "Submission Confirmation",
            Body = body,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(toEmail);
        await smtpClient.SendMailAsync(mailMessage);
    }
}