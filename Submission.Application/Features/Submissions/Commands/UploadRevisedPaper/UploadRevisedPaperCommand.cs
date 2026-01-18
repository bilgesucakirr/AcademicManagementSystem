using MediatR;
using Microsoft.AspNetCore.Http;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Commands.UploadRevisedPaper;

public record UploadRevisedPaperCommand(Guid SubmissionId, IFormFile File, FileType FileType) : IRequest<Unit>;