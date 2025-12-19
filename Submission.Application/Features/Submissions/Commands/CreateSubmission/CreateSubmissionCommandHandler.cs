using MediatR;
using Submission.Application.Contracts;
using Submission.Domain.Entities;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Commands.CreateSubmission;

public class CreateSubmissionCommandHandler : IRequestHandler<CreateSubmissionCommand, Guid>
{
    private readonly ISubmissionDbContext _context;
    private readonly IFileService _fileService;

    public CreateSubmissionCommandHandler(ISubmissionDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Guid> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        var submission = new Domain.Entities.Submission
        {
            Id = Guid.NewGuid(),
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
                SubmissionId = submission.Id
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
                SubmissionId = submission.Id
            });
        }

        await _context.Submissions.AddAsync(submission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return submission.Id;
    }
}