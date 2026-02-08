using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text;
using ToDo.Api.Common.Auth;
using ToDo.Api.Services.Interfaces;
using ToDo.Api.Services;
using ToDo.Api.Infrastructure.Data;
using ToDo.Api.Repository;

var builder = WebApplication.CreateBuilder(args);


// USER NEED: API endpoints
// DEV: Add controllers + swagger

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // USER NEED: Add JWT token in Swagger UI when testing protected endpoints.
    // DEV: Register Bearer security scheme and requirement.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste: Bearer {your JWT token}"
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


// USER NEED: Store data in PostgreSQL
// DEV: Register EF Core DbContext with connection string

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// USER NEED: JWT settings from configuration.
// DEV: Bind settings and register token generator.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenGenerator>();

// USER NEED: Auth logic (register/login)
// DEV: Register services for DI

builder.Services.AddScoped<IAuthService, AuthService>();

// USER NEED: CRUD tasks and subtasks without controllers touching DbContext.
// DEV: Register repositories for DI.
// WHY REPO/DTO: Controllers depend on repository interfaces and DTOs for clean separation.
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ISubTaskRepository, SubTaskRepository>();
builder.Services.AddScoped<ITimeLogRepository, TimeLogRepository>();


// USER NEED: Secure API with JWT
// DEV: Configure JWT Bearer authentication

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new Exception("JWT settings are missing. Add Jwt:Key, Jwt:Issuer, and Jwt:Audience in appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // USER NEED: Accept tokens pasted from Swagger even if formatting is wrong.
        // DEV: Normalize Authorization header so "Bearer {\"token\":\"...\"}" still works.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authHeader))
                {
                    return Task.CompletedTask;
                }

                if (authHeader.StartsWith("Bearer {", StringComparison.OrdinalIgnoreCase))
                {
                    var jsonStart = authHeader.IndexOf('{');
                    var json = authHeader.Substring(jsonStart);
                    try
                    {
                        using var doc = JsonDocument.Parse(json);
                        if (doc.RootElement.TryGetProperty("token", out var tokenElement))
                        {
                            context.Token = tokenElement.GetString();
                        }
                    }
                    catch (JsonException)
                    {
                        // Ignore and let default processing handle invalid tokens.
                    }
                }
                else if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                else if (authHeader.Count(c => c == '.') == 2)
                {
                    context.Token = authHeader.Trim();
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANT: Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
