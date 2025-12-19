using MediatR;
using Microsoft.AspNetCore.Http;
using Submission.Application.DTOs;
using Submission.Domain.Enums;

namespace Submission.Application.Features.Submissions.Commands.CreateSubmission;

public record CreateSubmissionCommand : IRequest<Guid>
{
    public Guid VenueId { get; set; }
    public Guid VenueEditionId { get; set; }
    public Guid CallForPapersId { get; set; }
    public Guid? TrackId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Abstract { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
    public SubmissionType Type { get; set; }

    public bool IsOriginal { get; set; }
    public bool IsNotElsewhere { get; set; }
    public bool HasConsent { get; set; }
    public bool HasConflictOfInterest { get; set; }
    public string? ConflictDetails { get; set; }

    public List<AuthorDto> Authors { get; set; } = new();

    public IFormFile? ManuscriptFile { get; set; }

    public Guid SubmitterId { get; set; }
}