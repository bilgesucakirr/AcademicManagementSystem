using MediatR;
using Microsoft.AspNetCore.Mvc;
using Review.Application.Features.Reviews.Commands.SubmitReview;

namespace Review.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // Bu, URL'in /api/reviews olmasını sağlar
public class ReviewsController : ControllerBase
{
    private readonly ISender _mediator;

    // MediatR'ın kendisini (ISender arayüzü ile) enjekte ediyoruz.
    // Bu, komutları ve sorguları göndermemizi sağlayan ana araçtır.
    public ReviewsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Bir hakem değerlendirmesini gönderir.
    /// </summary>
    /// <param name="assignmentId">Değerlendirmenin yapıldığı atama kimliği.</param>
    /// <param name="command">Değerlendirme verilerini içeren istek gövdesi.</param>
    /// <returns>Başarılı olduğunda 204 No Content döner.</returns>
    [HttpPost("{assignmentId:guid}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [IgnoreAntiforgeryToken] // <-- GEREKLİ DEĞİŞİKLİK BURADA!
    public async Task<IActionResult> SubmitReview(
        [FromRoute] Guid assignmentId,
        [FromForm] SubmitReviewCommand command)
    {
        // Gelen URL'deki assignmentId'yi komut nesnesinin içine yerleştiriyoruz.
        // Bu, verinin tek bir yerde toplanmasını sağlar.
        command.AssignmentId = assignmentId;

        // Komutu MediatR aracılığıyla ilgili Handler'a gönderiyoruz.
        // MediatR, doğru Handler'ı (SubmitReviewCommandHandler) kendisi bulur ve çalıştırır.
        await _mediator.Send(command);

        // İşlem başarılı olduğunda, HTTP 204 No Content durum kodunu döndürüyoruz.
        // Bu, "İşlemin başarılı oldu ama sana geri gönderecek bir içeriğim yok" demektir.
        // POST/PUT/DELETE işlemleri için yaygın bir kullanımdır.
        return NoContent();
    }
}