using MediatR;
using Microsoft.AspNetCore.Mvc;
using Submission.Application.Features.Integration;

namespace Submission.Api.Controllers;

[Route("api/integration")]
[ApiController]
public class IntegrationController : ControllerBase
{
    private readonly ISender _mediator;
    private const string ApiKey = "INTERNAL_SECRET_KEY_123";

    public IntegrationController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("submissions/{id}/review-stats")]
    public async Task<IActionResult> UpdateStats(Guid id, [FromBody] UpdateReviewStatsPayload payload)
    {
        if (!Request.Headers.TryGetValue("X-Internal-Key", out var key) || key != ApiKey)
            return Unauthorized();

        await _mediator.Send(new UpdateReviewStatsCommand(id, payload.AssignedDelta, payload.CompletedDelta));
        return Ok();
    }
}

public record UpdateReviewStatsPayload(int AssignedDelta, int CompletedDelta);