namespace Venue.Domain.Enums;

public enum BlindMode
{
    SingleBlind, // Hakem yazarı görür, yazar hakemi görmez
    DoubleBlind, // İkisi de birbirini görmez (Dosya temizliği şart)
    OpenReview   // Herkes herkesi görür
}