using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Review.Application.Contracts;
using Review.Infrastructure.Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlServer(connectionString,
        b => b.MigrationsAssembly(typeof(ReviewDbContext).Assembly.FullName)));

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<ReviewDbContext>());

builder.Services.AddCors(options =>
{
    options.AddPolicy("_myAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("https://localhost:7018")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var key = Encoding.ASCII.GetBytes("BuCokGizliVeUzunBirAnahtarOlmalı_EnAz32Karakter_2025_Jarvis");

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
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Review.Application.IAssemblyMarker).Assembly));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IFileService, Review.Api.Services.FileService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<Review.Api.Middleware.ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("_myAllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
