using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Review.Application.Features.Assignments.Commands.InviteReviewer;

namespace Review.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AssignmentsController : ControllerBase
{
    private readonly ISender _mediator;

    public AssignmentsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("invite")]
    [Authorize(Roles = "Admin,EditorInChief,TrackChair")]
    public async Task<IActionResult> InviteReviewer([FromBody] InviteReviewerCommand command)
    {
        var assignmentId = await _mediator.Send(command);
        return Ok(new
        {
            AssignmentId = assignmentId,
            Message = "Reviewer has been invited. Waiting for acceptance."
        });
    }
}