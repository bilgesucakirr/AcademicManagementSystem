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

    public CreateSubmissionCommandHandler(ISubmissionDbContext context, IFileService fileService, IEmailService emailService)
    {
        _context = context; _fileService = fileService; _emailService = emailService;
    }

    public async Task<Guid> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        if (request.ManuscriptFile != null && Path.GetExtension(request.ManuscriptFile.FileName).ToLower() != ".docx")
            throw new InvalidOperationException("Only .docx files are allowed.");

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
            SubmitterUserId = request.SubmitterId,
            Status = SubmissionStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            IsOriginal = request.IsOriginal,
            HasConsent = request.HasConsent,
            IsNotElsewhere = request.IsNotElsewhere
        };

        foreach (var author in request.Authors)
        {
            submission.Authors.Add(new Author { Id = Guid.NewGuid(), FirstName = author.FirstName, LastName = author.LastName, Email = author.Email, Affiliation = author.Affiliation, Country = author.Country, IsCorresponding = author.IsCorresponding, SubmissionId = submissionId });
        }

        if (request.ManuscriptFile != null)
        {
            var url = await _fileService.SaveFileAsync(request.ManuscriptFile);
            submission.Files.Add(new SubmissionFile { Id = Guid.NewGuid(), OriginalFileName = request.ManuscriptFile.FileName, StoragePath = url, Type = FileType.MainManuscript, SubmissionId = submissionId });
        }

        await _context.Submissions.AddAsync(submission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // MAİL BİLDİRİMLERİ
        try
        {
            if (!string.IsNullOrEmpty(request.SubmitterEmail))
                await _emailService.SendSubmissionReceiptAsync(request.SubmitterEmail, request.SubmitterName, request.Title);

            // ORGANİZATÖRE MAİL GÖNDERİMİ
            if (!string.IsNullOrEmpty(request.OrganizerEmail))
                await _emailService.SendSubmissionReceiptAsync(request.OrganizerEmail, "Organizer", $"[NEW SUBMISSION] {request.Title}");
        }
        catch { /* log error */ }

        return submission.Id;
    }
}