using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FramboyanSchedulerApi.Data;
using FramboyanSchedulerApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register email service (replace with real implementation for production)
builder.Services.AddScoped<FramboyanSchedulerApi.Services.IEmailService, FramboyanSchedulerApi.Services.FakeEmailService>();

// CORS Configuration - Environment specific
var isDevelopment = builder.Environment.IsDevelopment();
builder.Services.AddCors(options =>
{
    if (isDevelopment)
    {
        options.AddPolicy("AllowAll",
            policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    }
    else
    {
        var allowedOrigins = builder.Configuration.GetSection("SecuritySettings:AllowedOrigins").Get<string[]>() 
                           ?? new[] { "https://localhost:7043" };
        
        options.AddPolicy("ProductionCors", policy =>
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials());
    }
});

// Database Configuration - Environment specific
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Data Source=framboyan.db";
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(connectionString));

// Identity configuration - Enhanced for production
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = isDevelopment ? 6 : 12;
    options.Password.RequiredUniqueChars = isDevelopment ? 1 : 3;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
    
    // Sign in settings
    options.SignIn.RequireConfirmedEmail = !isDevelopment;
})
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// JWT authentication - Enhanced security
var jwtKey = builder.Configuration["JwtSettings:SecretKey"] ?? builder.Configuration["JwtKey"];
if (string.IsNullOrEmpty(jwtKey) || jwtKey == "super_secret_jwt_key_123!")
{
    if (!isDevelopment)
        throw new InvalidOperationException("JWT key must be configured for production");
    jwtKey = "super_secret_jwt_key_123!"; // Development fallback
}

var key = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = !isDevelopment;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = !isDevelopment,
        ValidateAudience = !isDevelopment,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ClockSkew = TimeSpan.FromMinutes(5),
        ValidateLifetime = true
    };
});

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger - Development only
if (isDevelopment)
{
    builder.Services.AddSwaggerGen();
}

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AuthDbContext>();

var app = builder.Build();

// Auto-migrate database and seed roles on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
    var roleTask = FramboyanSchedulerApi.Data.RoleSeeder.SeedRolesAsync(scope.ServiceProvider);
    roleTask.Wait();
}

// Configure middleware pipeline
if (isDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Production security headers
    app.UseHsts();
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        await next();
    });
}

app.UseHttpsRedirection();

// Use appropriate CORS policy
var corsPolicy = isDevelopment ? "AllowAll" : "ProductionCors";
app.UseCors(corsPolicy);

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();