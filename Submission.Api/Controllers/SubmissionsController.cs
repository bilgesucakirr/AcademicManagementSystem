using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Submission.Application.Features.Submissions.Commands.CreateSubmission;
using Submission.Application.Features.Submissions.Queries.GetMySubmissions; 
using System.Security.Claims;
using Submission.Application.Features.Submissions.Queries.GetSubmissionsList;


namespace Submission.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Sadece giriş yapmış kullanıcılar makale gönderebilir
public class SubmissionsController : ControllerBase
{
    private readonly ISender _mediator;

    public SubmissionsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateSubmissionCommand command)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            command.SubmitterId = userId;
        }
        else
        {
            return Unauthorized("User ID not found in token.");
        }
        var submissionId = await _mediator.Send(command);
        return Ok(new { Id = submissionId, Message = "Submission received successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> GetMySubmissions()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetMySubmissionsQuery(userId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin, EditorInChief, TrackChair")]
    public async Task<IActionResult> GetAllSubmissions()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        var trackIdClaim = User.FindFirst("AssignedTrackId")?.Value;
        int? trackId = trackIdClaim != null ? int.Parse(trackIdClaim) : null;

        var query = new GetSubmissionsListQuery(roleClaim!, trackId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}