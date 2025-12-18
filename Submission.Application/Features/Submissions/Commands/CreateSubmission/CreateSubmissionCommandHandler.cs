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
            Title = request.Title,
            Abstract = request.Abstract,
            Keywords = request.Keywords,
           
            ConferenceId = request.ConferenceId,
            JournalId = request.JournalId,
            SubmitterUserId = request.SubmitterId,
            CreatedAt = DateTime.UtcNow,
            Status = SubmissionStatus.Submitted
        };

        // Yazarları ekle
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

        // Dosya varsa kaydet
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