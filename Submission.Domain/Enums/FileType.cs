namespace Submission.Domain.Enums;

public enum FileType
{
    MainManuscript,      // Orijinal dosya (İsimli olabilir)
    AnonymousManuscript, // Kör hakemlik için (İsimsiz) - ZORUNLU OLABİLİR
    Supplementary,       // Ek dosyalar (Veri setleri vb.)
    CoverLetter          // Editöre mektup
}