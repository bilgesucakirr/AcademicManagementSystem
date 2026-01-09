namespace Submission.Application.Contracts;

public interface IEmailService
{
    Task SendSubmissionReceiptAsync(string toEmail, string authorName, string title);
}