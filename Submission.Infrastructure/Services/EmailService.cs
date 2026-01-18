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
        var subject = $"Submission Confirmation: {title}";
        var body = GetHtmlTemplate("Submission Received", $@"
            <p>Dear {authorName},</p>
            <p>Thank you for submitting your manuscript <strong>'{title}'</strong> to our platform.</p>
            <p>Your submission has been successfully received and is currently being processed for technical check.</p>
            <p>You can track the status of your submission through your dashboard.</p>");

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendDecisionEmailAsync(string toEmail, string authorName, string title, string status, string decisionLetter)
    {
        string color = status.Contains("Rejected") ? "#dc3545" : "#198754";
        if (status.Contains("Revision")) color = "#0dcaf0";

        var subject = $"Decision on your manuscript: {title}";
        var body = GetHtmlTemplate("Editorial Decision", $@"
            <p>Dear {authorName},</p>
            <p>Regarding your submission <strong>'{title}'</strong>,</p>
            <div style='padding: 20px; background: #f8f9fa; border-left: 4px solid {color}; margin: 20px 0;'>
                <h4 style='color: {color}; margin-top: 0; text-transform: uppercase;'>Result: {status}</h4>
                <p style='color: #334155; white-space: pre-wrap;'>{decisionLetter}</p>
            </div>
            <p>Please log in to the JARVIS portal to view detailed reviewer comments and take the necessary actions.</p>");

        await SendEmailAsync(toEmail, subject, body);
    }

    private string GetHtmlTemplate(string title, string content)
    {
        return $@"
        <div style='font-family: Segoe UI, Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e2e8f0; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05);'>
            <div style='background: #0f172a; padding: 25px; text-align: center;'>
                <h2 style='color: white; margin: 0; letter-spacing: 2px;'>JARVIS</h2>
                <div style='color: #94a3b8; font-size: 12px; margin-top: 5px; text-transform: uppercase;'>Academic Management System</div>
            </div>
            <div style='padding: 40px; line-height: 1.6; color: #334155;'>
                <h3 style='color: #0f172a; border-bottom: 2px solid #f1f5f9; padding-bottom: 15px; margin-top: 0;'>{title}</h3>
                {content}
                <div style='margin-top: 40px; text-align: center;'>
                    <a href='https://localhost:7018/dashboard' style='background: #3b82f6; color: white; padding: 14px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block;'>Go to Dashboard</a>
                </div>
            </div>
            <div style='background: #f8fafc; padding: 20px; text-align: center; font-size: 11px; color: #64748b; border-top: 1px solid #f1f5f9;'>
                &copy; 2026 JARVIS Academic Management. This is an automated notification.
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