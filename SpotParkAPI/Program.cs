﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SpotParkAPI.Middleware;
using SpotParkAPI.Models;
using SpotParkAPI.Repositories;
using SpotParkAPI.Repositories.Interfaces;
using SpotParkAPI.Services;
using SpotParkAPI.Services.Interfaces;
using System.Text;
using SpotParkAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Adaugă serviciile în container
builder.Services.AddControllers();

// Înregistrează DbContext-ul în DI container
builder.Services.AddDbContext<SpotParkDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    ));

// Înregistrează repository-uri și servicii
builder.Services.AddScoped<IParkingRepository, ParkingRepository>();
builder.Services.AddScoped<ParkingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<ICommonService, CommonService>();
builder.Services.AddScoped<IParkingImageRepository, ParkingImageRepository>();
builder.Services.AddScoped<ParkingImageService>();
builder.Services.AddScoped<IWalletService, WalletService>();

builder.Services.AddScoped<IWalletRepository, WalletRepository>();


builder.Services.AddHttpContextAccessor(); // Pentru IHttpContextAccessor





// Configurează autentificarea JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMobile", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
});

builder.Services.AddAuthorization();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowMobile");
app.UseStaticFiles();
app.UseMiddleware<ExceptionMiddleware>();

//app.UseHttpsRedirection();

app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers();

app.Run();


using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SpotParkDbContext>();
    try
    {
        dbContext.Database.OpenConnection();
        Console.WriteLine("Conexiunea la baza de date a reușit!");
        dbContext.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Eroare la conectare: {ex.Message}");
    }
}