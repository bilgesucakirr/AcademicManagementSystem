using MediatR;
using Microsoft.EntityFrameworkCore;
using Review.Application.Contracts; // IApplicationDbContext ve IFileService için
using Review.Domain.Enums;

namespace Review.Application.Features.Reviews.Commands.SubmitReview;

public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService; // DOSYA SERVİSİ İÇİN ARAYÜZ

    // CONSTRUCTOR GÜNCELLEMESİ: Artık IFileService'i de enjekte ediyoruz.
    public SubmitReviewCommandHandler(IApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<Unit> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
    {
        // 1. İlgili hakem atamasını veritabanında bul (Bu kısım aynı).
        var assignment = await _context.ReviewAssignments
            .FirstOrDefaultAsync(ra => ra.Id == request.AssignmentId, cancellationToken);

        if (assignment is null)
        {
            throw new KeyNotFoundException($"Assignment with ID {request.AssignmentId} not found.");
        }

        // 2. Domain kuralını kontrol et (Bu kısım aynı).
        if (assignment.Status != ReviewAssignmentStatus.Accepted)
        {
            throw new InvalidOperationException("Cannot submit a review for an assignment that is not in 'Accepted' status.");
        }

        // 3. YENİ ADIM: Eğer bir dosya gönderildiyse, onu kaydet ve URL'ini al.
        string? attachmentUrl = null;
        if (request.ReviewFile is not null && request.ReviewFile.Length > 0)
        {
            // Dosya servisini kullanarak dosyayı kaydet ve erişim adresini al.
            attachmentUrl = await _fileService.SaveReviewFileAsync(request.ReviewFile);
        }

        // 4. Yeni bir Review domain varlığı oluştur (Constructor güncellendi).
        var newReview = new Domain.Entities.Review(
            request.AssignmentId,
            request.OverallScore,
            request.Confidence,
            request.CommentsToAuthor,
            request.CommentsToEditor,
            attachmentUrl // <-- Kaydedilen dosyanın URL'i buraya veriliyor.
        );

        // 5. Domain varlığının durumunu güncelle (Bu kısım aynı).
        assignment.MarkAsSubmitted();

        // 6. Yeni Review'u DbContext'e ekle (Bu kısım aynı).
        await _context.Reviews.AddAsync(newReview, cancellationToken);

        // 7. Tüm değişiklikleri atomik bir işlemle veritabanına kaydet (Bu kısım aynı).
        await _context.SaveChangesAsync(cancellationToken);

        // Geriye bir değer dönmeyeceğimizi belirtiyoruz.
        return Unit.Value;
    }
}