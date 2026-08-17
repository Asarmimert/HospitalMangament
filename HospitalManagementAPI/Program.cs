using HospitalManagement.Business.Somut;
using HospitalManagement.Business.Soyut;
using HospitalManagement.DataAccess.Context;
using HospitalManagement.DataAccess.Depolar.Somut;
using HospitalManagement.DataAccess.Depolar.Soyut;
using HospitalManagementAPI.Middleware;
using Microsoft.EntityFrameworkCore;
using HospitalManagementAPI.Filters;
using HospitalManagement.Entity.Entities;
using HospitalManagementAPI.Ayarlar;
using HospitalManagementAPI.Servisler;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "DefaultConnection bağlantı bilgisi bulunamadı.");
}

// PostgreSQL ve DbContext kaydı
builder.Services.AddDbContext<HospitalDbContext>(
    options =>
    {
        options.UseNpgsql(connectionString);
    });
// Doktora özel depo kaydı
builder.Services.AddScoped<
    IDoktorDeposu,
    DoktorDeposu>();
// Hastaya özel depo kaydı
builder.Services.AddScoped<
    IHastaDeposu,
    HastaDeposu>();

// Genel repository kaydı
builder.Services.AddScoped(
    typeof(IGenelDepo<>),
    typeof(GenelDepo<>));
builder.Services.AddScoped<
    IReceteIcerikDeposu,
    ReceteIcerikDeposu>();
builder.Services.AddScoped<
    IReceteIcerikServisi,
    ReceteIcerikServisi>();
// Departman servisi kaydı
builder.Services.AddScoped<
    IDepartmanServisi,
    DepartmanServisi>();
builder.Services.AddScoped<
    IIlacServisi,
    IlacServisi>();
builder.Services.AddScoped<IslemLoglamaFiltresi>();
// Controller kayıtları
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<IslemLoglamaFiltresi>();
});
// Aktif departman listesi için cache
builder.Services.AddMemoryCache();

// Yetkilendirme servisi
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy =
        new Microsoft.AspNetCore.Authorization
            .AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});
// Swagger servisleri
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "bearer",
        new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT token değerini giriniz."
        });

    options.AddSecurityRequirement(
        document => new OpenApiSecurityRequirement
        {
            [
                new OpenApiSecuritySchemeReference(
                    "bearer",
                    document)
            ] = []
        });
});

builder.Services.AddScoped<
    IDoktorServisi,
    DoktorServisi>();


// Hasta servisi kaydı
builder.Services.AddScoped<
    IHastaServisi,
    HastaServisi>();
builder.Services.AddScoped<
    ISekreterServisi,
    SekreterServisi>();
// Randevuya özel depo kaydı
builder.Services.AddScoped<
    IRandevuDeposu,
    RandevuDeposu>();

// Randevu servisi kaydı
builder.Services.AddScoped<
    IRandevuServisi,
    RandevuServisi>();

// Muayeneye özel depo kaydı
builder.Services.AddScoped<
    IMuayeneDeposu,
    MuayeneDeposu>();
// Muayene servisi kaydı
builder.Services.AddScoped<
    IMuayeneServisi,
    MuayeneServisi>();
// Teşhis servisi kaydı
builder.Services.AddScoped<
    ITeshisServisi,
    TeshisServisi>();
// Muayene teşhisine özel depo kaydı
builder.Services.AddScoped<
    IMuayeneTeshisiDeposu,
    MuayeneTeshisiDeposu>();
builder.Services.AddScoped<
    IMuayeneTeshisiServisi,
    MuayeneTeshisiServisi>();
builder.Services.AddScoped<
    IReceteDeposu,
    ReceteDeposu>();
builder.Services.AddScoped<
    IReceteServisi,
    ReceteServisi>();
builder.Services.AddScoped<
    IKullaniciHesabiServisi,
    KullaniciHesabiServisi>();
// JWT ayarlarını appsettings.json dosyasından okur
builder.Services.Configure<JwtAyarlari>(
    builder.Configuration.GetSection(
        JwtAyarlari.BolumAdi));

// JWT oluşturma servisi
builder.Services.AddScoped<
    IJwtTokenServisi,
    JwtTokenServisi>();

// Parolaları güvenli şekilde hashleme servisi
builder.Services.AddScoped<
    IPasswordHasher<KullaniciHesabi>,
    PasswordHasher<KullaniciHesabi>>();
var jwtAyarlari = builder.Configuration
    .GetSection(JwtAyarlari.BolumAdi)
    .Get<JwtAyarlari>()
    ?? throw new InvalidOperationException(
        "JWT ayarları bulunamadı.");

if (string.IsNullOrWhiteSpace(jwtAyarlari.Anahtar))
{
    throw new InvalidOperationException(
        "JWT anahtarı bulunamadı.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtAyarlari.Veren,
                ValidAudience = jwtAyarlari.Hedef,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtAyarlari.Anahtar)),

                ClockSkew = TimeSpan.Zero
            };
    });
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<HataYonetimiMiddleware>();
// Merkezi hata yönetimi
app.UseHttpsRedirection();

// wwwroot içindeki index.html dosyasını başlangıç sayfası yapar.
app.UseDefaultFiles();

// wwwroot içindeki HTML, CSS ve JavaScript dosyalarını yayınlar.
app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();