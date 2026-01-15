using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string Interests { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }

    // YENİ EKLENEN ALANLAR
    public string? Affiliation { get; set; } // Kurum / Üniversite
    public string? Country { get; set; }     // Ülke
    public string? Biography { get; set; }   // Kısa Biyografi
    public string? Title { get; set; }       // Ünvan (Prof., Dr., vb.)
}