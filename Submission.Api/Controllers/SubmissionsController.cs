using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Submission.Application.Features.Submissions.Commands.CreateSubmission;
using System.Security.Claims;

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
}