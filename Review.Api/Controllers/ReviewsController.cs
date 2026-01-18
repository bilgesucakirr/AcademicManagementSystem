using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Review.Application.Features.Reviews.Commands.SubmitReview;
using Review.Application.Features.Reviews.Queries.GetMyAssignments;
using System.Security.Claims;

namespace Review.Api.Controllers;

[ApiController]
[Route("api/[controller]")] 
public class ReviewsController : ControllerBase
{
    private readonly ISender _mediator;

   
    public ReviewsController(ISender mediator)
    {
        _mediator = mediator;
    }

  
    /// <param name="assignmentId">Değerlendirmenin yapıldığı atama kimliği.</param>
    /// <param name="command">Değerlendirme verilerini içeren istek gövdesi.</param>
    /// <returns>Başarılı olduğunda 204 No Content döner.</returns>
    [HttpPost("{assignmentId:guid}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [IgnoreAntiforgeryToken] 
    public async Task<IActionResult> SubmitReview(
        [FromRoute] Guid assignmentId,
        [FromForm] SubmitReviewCommand command)
    {
        
        command.AssignmentId = assignmentId;
        await _mediator.Send(command);

     
        return NoContent();
    }

    [HttpGet("my-assignments")]
    [Authorize(Roles = "Reviewer")]
    public async Task<IActionResult> GetMyAssignments()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var query = new GetMyAssignmentsQuery(userId);
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("submission/{submissionId}/anonymous")]
    public async Task<IActionResult> GetAnonymousReviews(Guid submissionId)
    {
        return Ok(await _mediator.Send(new Review.Application.Features.Reviews.Queries.GetSubmissionReviews.GetAnonymousReviewsQuery(submissionId)));
    }
}