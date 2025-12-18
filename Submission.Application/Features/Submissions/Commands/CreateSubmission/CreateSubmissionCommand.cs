using MediatR;
using Microsoft.AspNetCore.Http;
using Submission.Application.DTOs;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Commands.CreateSubmission;

public record CreateSubmissionCommand : IRequest<Guid>
{
    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;

    public SubmissionContext Context { get; set; }
    public int? ConferenceId { get; set; }
    public int? JournalId { get; set; }

    public List<AuthorDto> Authors { get; set; } = new();

    public IFormFile? ManuscriptFile { get; set; }

    public Guid SubmitterId { get; set; } // Controller'da doldurulacak
}