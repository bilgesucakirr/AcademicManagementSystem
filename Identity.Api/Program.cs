using Identity.Api.Data;
using Identity.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorUI",
        policy =>
        {
            
            policy.WithOrigins("https://localhost:7018")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // ŞİFRE ZORLUK AYARLARINI TAMAMEN KAPATIYORUZ
    options.Password.RequireDigit = false;           // Rakam zorunlu değil
    options.Password.RequireLowercase = false;       // Küçük harf zorunlu değil
    options.Password.RequireUppercase = false;       // Büyük harf zorunlu değil
    options.Password.RequireNonAlphanumeric = false; // Sembol (!, @) zorunlu değil
    options.Password.RequiredLength = 3;             // En az 3 karakter olsun yeter
    options.Password.RequiredUniqueChars = 0;        // Benzersiz karakter zorunluluğu YOK (Sorunu çözen kısım)
})
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddScoped<Identity.Api.Services.IAuthService, Identity.Api.Services.AuthService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazorUI");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();