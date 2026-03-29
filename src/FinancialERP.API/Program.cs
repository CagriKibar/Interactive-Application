using FinancialERP.API.BackgroundServices;
using FinancialERP.API.Middleware;
using FinancialERP.Business.Mapping;
using FinancialERP.DataAccess.BarsoftIntegration;
using FinancialERP.DataAccess.Context;
using FinancialERP.DataAccess.Repositories.Abstract;
using FinancialERP.DataAccess.Repositories.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === Veritabanı Bağlantıları ===
// Ana veritabanı (PostgreSQL)
builder.Services.AddDbContext<FinancialDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Barsoft ERP veritabanı (MSSQL - Salt Okunur)
builder.Services.AddDbContext<BarsoftDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BarsoftReadOnly")));

// === Repository'ler ===
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<IBarsoftDataService, BarsoftDataService>();

// === Business Servisleri ===
builder.Services.AddBusinessServices();

// === HTTP Client (TCMB API için) ===
builder.Services.AddHttpClient("TCMB", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/xml");
});
builder.Services.AddHttpClient("EVDS", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// === Background Services ===
builder.Services.AddHostedService<DailyDataSyncService>();
builder.Services.AddHostedService<RealTimeAnalysisService>();

// === CORS (Electron Frontend) ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("ElectronApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "app://.")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Financial ERP AI API", Version = "v1" });
});

var app = builder.Build();

// === Middleware Pipeline ===
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ElectronApp");
app.MapControllers();
app.Run();
