namespace Review.Application.Contracts;

public interface IEmailService
{
    Task SendInvitationEmailAsync(string toEmail, string assignmentId, string submissionTitle);
}