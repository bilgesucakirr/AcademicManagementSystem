using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Submission.Application.Features.Submissions.Commands.CreateSubmission;
using Submission.Application.Features.Submissions.Commands.FinalizeSubmission;
using Submission.Application.Features.Submissions.Commands.RecordDecision;
using Submission.Application.Features.Submissions.Queries.GetMySubmissions;
using Submission.Application.Features.Submissions.Queries.GetSubmissionsList;
using System.Security.Claims;

namespace Submission.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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

        Guid? trackId = null;
        if (!string.IsNullOrEmpty(trackIdClaim) && Guid.TryParse(trackIdClaim, out var parsedGuid))
        {
            trackId = parsedGuid;
        }

        var query = new GetSubmissionsListQuery(roleClaim!, trackId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new Submission.Application.Features.Submissions.Queries.GetSubmissionDetail.GetSubmissionDetailQuery(id);
        var result = await _mediator.Send(query);

        if (result == null) return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

       

        return Ok(result);
    }

    [HttpPost("{id}/decision")]
    [Authorize(Roles = "Admin,EditorInChief,TrackChair")]
    public async Task<IActionResult> RecordDecision(Guid id, [FromBody] RecordDecisionCommand command)
    {
        command.SubmissionId = id;
        await _mediator.Send(command);
        return Ok(new { Message = "Decision recorded and author notified." });
    }

    [HttpPost("{id}/finalize")]
    [Authorize]
    public async Task<IActionResult> Finalize(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var refNo = await _mediator.Send(new FinalizeSubmissionCommand(id, userId));
        return Ok(new { ReferenceNumber = refNo });
    }
}