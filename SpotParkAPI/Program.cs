using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
using SpotParkAPI.Repositories;
using SpotParkAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Înregistreaz? DbContext-ul în DI container
builder.Services.AddDbContext<SpotParkDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adaug? serviciile necesare pentru aplica?ie
builder.Services.AddControllers();
builder.Services.AddScoped<IParkingRepository, ParkingRepository>();
builder.Services.AddScoped<ParkingService>();



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
