using MediatR;
using Submission.Application.Contracts;
using Submission.Domain.Entities;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Commands.CreateSubmission;

public class CreateSubmissionCommandHandler : IRequestHandler<CreateSubmissionCommand, Guid>
{
    private readonly ISubmissionDbContext _context;
    private readonly IFileService _fileService;
    private readonly IEmailService _emailService;

    public CreateSubmissionCommandHandler(
        ISubmissionDbContext context,
        IFileService fileService,
        IEmailService emailService)
    {
        _context = context;
        _fileService = fileService;
        _emailService = emailService;
    }

    public async Task<Guid> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submissionId = Guid.NewGuid();

        var submission = new Domain.Entities.Submission
        {
            Id = submissionId,
            VenueId = request.VenueId,
            VenueEditionId = request.VenueEditionId,
            CallForPapersId = request.CallForPapersId,
            TrackId = request.TrackId,
            Title = request.Title,
            Abstract = request.Abstract,
            Keywords = request.Keywords,
            Type = request.Type,
            IsOriginal = request.IsOriginal,
            IsNotElsewhere = request.IsNotElsewhere,
            HasConsent = request.HasConsent,
            HasConflictOfInterest = request.HasConflictOfInterest,
            ConflictDetails = request.ConflictDetails,
            SubmitterUserId = request.SubmitterId,
            CreatedAt = DateTime.UtcNow,
            Status = SubmissionStatus.Draft
        };

        foreach (var authorDto in request.Authors)
        {
            submission.Authors.Add(new Author
            {
                Id = Guid.NewGuid(),
                FirstName = authorDto.FirstName,
                LastName = authorDto.LastName,
                Email = authorDto.Email,
                Affiliation = authorDto.Affiliation,
                Country = authorDto.Country,
                IsCorresponding = authorDto.IsCorresponding,
                SubmissionId = submissionId
            });
        }

        if (request.ManuscriptFile != null)
        {
            var fileUrl = await _fileService.SaveFileAsync(request.ManuscriptFile);

            submission.Files.Add(new SubmissionFile
            {
                Id = Guid.NewGuid(),
                OriginalFileName = request.ManuscriptFile.FileName,
                StoragePath = fileUrl,
                Type = FileType.MainManuscript,
                SubmissionId = submissionId
            });
        }

        await _context.Submissions.AddAsync(submission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            // 1. ADIM: İşlemi başlatan kişiye (Bilgesu Çakır) mail gönder
            if (!string.IsNullOrEmpty(request.SubmitterEmail))
            {
                await _emailService.SendSubmissionReceiptAsync(
                    request.SubmitterEmail,
                    request.SubmitterName,
                    request.Title);
            }

            // 2. ADIM: Formdaki yazar listesindeki her bir yazara mail gönder
            foreach (var author in request.Authors)
            {
                // Eğer bu yazarın maili, işlemi yapan kişinin mailinden farklıysa mail at
                if (!string.Equals(author.Email, request.SubmitterEmail, StringComparison.OrdinalIgnoreCase))
                {
                    // DÜZELTME: request.SubmitterName yerine author.FirstName + author.LastName kullanıyoruz
                    string fullName = $"{author.FirstName} {author.LastName}";

                    await _emailService.SendSubmissionReceiptAsync(
                        author.Email,
                        fullName,
                        request.Title);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MAIL NOTIFY ERROR] {ex.Message}");
        }

        return submission.Id;
    }
}