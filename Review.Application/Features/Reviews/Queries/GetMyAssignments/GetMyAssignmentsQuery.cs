using MediatR;
using Review.Application.DTOs;

namespace Review.Application.Features.Reviews.Queries.GetMyAssignments;

public record GetMyAssignmentsQuery(Guid ReviewerId) : IRequest<List<ReviewAssignmentDto>>;