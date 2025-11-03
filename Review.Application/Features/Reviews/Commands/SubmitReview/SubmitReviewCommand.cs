using MediatR;

namespace Review.Application.Features.Reviews.Commands.SubmitReview;

/// <summary>
/// Bir hakem değerlendirmesini gönderme işlemini temsil eden komut.
/// IRequest<Unit>, bu komutun geriye bir değer döndürmeyeceğini belirtir.
/// </summary>
public record SubmitReviewCommand : IRequest<Unit>
{
    /// <summary>
    /// Değerlendirmenin yapıldığı atamanın kimliği (URL'den gelecek).
    /// </summary>
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// Hakemin verdiği genel puan (örn: 1-5 arası).
    /// </summary>
    public decimal OverallScore { get; set; }

    /// <summary>
    /// Hakemin değerlendirmesine olan güveni (örn: 1-5 arası).
    /// </summary>
    public int Confidence { get; set; }

    /// <summary>
    /// Yazara iletilecek yorumlar.
    /// </summary>
    public string CommentsToAuthor { get; set; } = string.Empty;

    /// <summary>
    /// Sadece editöre iletilecek özel yorumlar (opsiyonel).
    /// </summary>
    public string? CommentsToEditor { get; set; }
}