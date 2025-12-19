namespace Review.Domain.Enums;

public enum ReviewAssignmentStatus
{
    Created,        // Atama taslağı (belki sistem otomatik oluşturdu)
    Invited,        // Hakeme davet gönderildi, cevap bekleniyor
    Accepted,       // Hakem kabul etti, değerlendirme başladı
    Declined,       // Hakem reddetti
    Expired,        // Hakem süresi içinde cevap vermedi
    Submitted,      // Değerlendirme tamamlandı
    Completed,      // Editör değerlendirmeyi onayladı/kullandı
    Overdue         // Süresi geçti
}