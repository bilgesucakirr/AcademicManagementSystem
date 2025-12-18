using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Review.Api.Services;
using Review.Application.Contracts;
using Review.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<IApplicationDbContext, ReviewDbContext>(options =>
    options.UseSqlServer(connectionString,
        b => b.MigrationsAssembly(typeof(ReviewDbContext).Assembly.FullName)));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Review.Application.IAssemblyMarker).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Review.Application.IAssemblyMarker).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // BURASI ÇOK ÖNEMLİ: UI Projenin çalıştığı port (7018) olmalı
                          policy.WithOrigins("https://localhost:7018")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IFileService, FileService>();

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

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ReviewDbContext>();
        await SeedData.InitializeDatabaseAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı seed işlemi sırasında bir hata oluştu.");
    }
}

app.Run();