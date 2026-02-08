using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ToDo.Api.Services.Interfaces;
using ToDo.Api.Services;
using ToDo.Api.Infrastructure.Data;
using ToDo.Api.Repository;

var builder = WebApplication.CreateBuilder(args);


// USER NEED: API endpoints
// DEV: Add controllers + swagger

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// USER NEED: Store data in PostgreSQL
// DEV: Register EF Core DbContext with connection string

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);


// USER NEED: Auth logic (register/login)
// DEV: Register services for DI

builder.Services.AddScoped<IAuthService, AuthService>();

// USER NEED: CRUD tasks and subtasks without controllers touching DbContext.
// DEV: Register repositories for DI.
// WHY REPO/DTO: Controllers depend on repository interfaces and DTOs for clean separation.
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ISubTaskRepository, SubTaskRepository>();


// USER NEED: Secure API with JWT
// DEV: Configure JWT Bearer authentication

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("JWT Key is missing. Add Jwt:Key in appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
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
