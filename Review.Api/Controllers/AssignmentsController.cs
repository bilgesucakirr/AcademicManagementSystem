using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Review.Application.Features.Assignments.Commands.AcceptInvitation;
using Review.Application.Features.Assignments.Commands.DeclineInvitation;
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
        return Ok(new { AssignmentId = assignmentId, Message = "Reviewer has been invited. Waiting for acceptance." });
    }

    [HttpPost("{id}/accept")]
    [Authorize(Roles = "Reviewer,Admin")]
    public async Task<IActionResult> AcceptInvitation(Guid id)
    {
        await _mediator.Send(new AcceptInvitationCommand(id));
        return Ok(new { Message = "Invitation accepted successfully. You can now start the review." });
    }

    [HttpPost("{id}/decline")]
    [Authorize(Roles = "Reviewer,Admin")]
    public async Task<IActionResult> DeclineInvitation(Guid id, [FromBody] string reason)
    {
        await _mediator.Send(new DeclineInvitationCommand(id, reason));
        return Ok(new { Message = "Invitation declined." });
    }
}