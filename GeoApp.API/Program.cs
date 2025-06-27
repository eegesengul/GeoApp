using AutoMapper;
using GeoApp.Application.Features.Areas.Commands;
using GeoApp.Application.Features.Areas.Handlers;
using GeoApp.Application.Interfaces;
using GeoApp.Application.Mappings;
using GeoApp.Infrastructure.Entities;
using GeoApp.Infrastructure.Mappings;
using GeoApp.Infrastructure.Persistence;
using GeoApp.Infrastructure.Services;
using GeoApp.Infrastructure.Settings;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization; // <-- 1. YENÝ EKLENEN USING

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseNetTopologySuite()));

// ApplicationDbContext -> IApplicationDbContext binding
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

// Identity - AppUser kullanýlmalý!
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Settings + Token Service
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// JWT ayarlarýný al
var jwtSettings = builder.Configuration.GetSection("JWT").Get<JWTSettings>()
    ?? throw new InvalidOperationException("JWT ayarlarý yüklenemedi.");

var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

// JWT middleware
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
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// AutoMapper
builder.Services.AddAutoMapper(
    typeof(GeoApp.Application.Mappings.UserMappingProfile).Assembly,
    typeof(GeoApp.Application.Mappings.AreaMappingProfile).Assembly,
    typeof(GeoApp.Infrastructure.Mappings.AppUserMappingProfile).Assembly
);

// MediatR - 2. DÜZENLENEN KISIM
// Bu tek satýr, GeoApp.Application projesindeki TÜM handler'larý (Area, Point vb.)
// otomatik olarak bulur ve kaydeder. Daha temiz ve geleceðe dönük bir yaklaþýmdýr.
builder.Services.AddMediatR(typeof(CreateAreaCommandHandler).Assembly);


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger + JWT Auth
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "GeoApp API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header. Örnek: 'Bearer {token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Controllers - 3. GÜNCELLENEN KISIM
// AddControllers() metoduna AddJsonOptions ekleyerek 500 hatasýný çözüyoruz.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // JSON'a çevirme sýrasýnda "Infinity" gibi özel sayý deðerlerine izin verir.
    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
});


var app = builder.Build();

// Rol ve admin kullanýcý seedleme
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    string[] roles = new[] { "USER", "ADMIN" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
        }
    }

    var adminEmail = "admin@admin.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var newAdmin = new AppUser
        {
            UserName = "admin",
            Email = adminEmail
        };

        var result = await userManager.CreateAsync(newAdmin, "ege123ege");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "ADMIN");
            Console.WriteLine("Admin kullanýcýsý baþarýyla oluþturuldu.");
        }
        else
        {
            Console.WriteLine("Admin oluþturulamadý: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();