using FluentValidation;

namespace Review.Application.Features.Reviews.Commands.SubmitReview;

public class SubmitReviewCommandValidator : AbstractValidator<SubmitReviewCommand>
{
    public SubmitReviewCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty().WithMessage("Atama kimliği boş olamaz.");

        RuleFor(x => x.OverallScore)
            .InclusiveBetween(1, 5).WithMessage("Puan 1 ile 5 arasında olmalıdır.");

        RuleFor(x => x.Confidence)
            .InclusiveBetween(1, 5).WithMessage("Güven seviyesi 1 ile 5 arasında olmalıdır.");

        RuleFor(x => x.CommentsToAuthor)
            .NotEmpty().WithMessage("Yazar için yorumlar boş bırakılamaz.")
            .MinimumLength(50).WithMessage("Yazar yorumları en az 50 karakter olmalıdır.");
    }
}