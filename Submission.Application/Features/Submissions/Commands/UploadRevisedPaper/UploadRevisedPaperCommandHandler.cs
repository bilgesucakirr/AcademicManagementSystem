using MediatR;
using Microsoft.EntityFrameworkCore;
using Submission.Application.Contracts;
using Submission.Domain.Entities;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Commands.UploadRevisedPaper;

public class UploadRevisedPaperCommandHandler : IRequestHandler<UploadRevisedPaperCommand, Unit>
{
    private readonly ISubmissionDbContext _context;
    private readonly IFileService _fileService;
    private readonly IEmailService _emailService;

    public UploadRevisedPaperCommandHandler(ISubmissionDbContext context, IFileService fileService, IEmailService emailService)
    {
        _context = context;
        _fileService = fileService;
        _emailService = emailService;
    }

    public async Task<Unit> Handle(UploadRevisedPaperCommand request, CancellationToken cancellationToken)
    {
        var sub = await _context.Submissions.Include(s => s.Authors).FirstOrDefaultAsync(s => s.Id == request.SubmissionId);
        if (sub == null) throw new Exception("Not found");

        var url = await _fileService.SaveFileAsync(request.File);
        _context.SubmissionFiles.Add(new SubmissionFile
        {
            Id = Guid.NewGuid(),
            SubmissionId = sub.Id,
            StoragePath = url,
            OriginalFileName = request.File.FileName,
            Type = request.FileType
        });

        if (request.FileType == FileType.MainManuscript)
        {
            sub.Status = SubmissionStatus.UnderReview;
            using var client = new HttpClient();
            await client.PostAsync($"https://localhost:7047/api/Assignments/submission/{sub.Id}/re-activate", null);
        }
        else if (request.FileType == FileType.CameraReady)
        {
            sub.Status = SubmissionStatus.CameraReadySubmitted;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}