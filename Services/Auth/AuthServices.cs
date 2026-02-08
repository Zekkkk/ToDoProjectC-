using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ToDo.Api.DTO.Auth;
using ToDo.Api.Infrastructure.Data;
using ToDo.Api.Services.Interfaces;
using ToDo.Api.Domain.Entities;

namespace ToDo.Api.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var username = dto.Username.Trim().ToLower();

        var exists = await _db.Users.AnyAsync(u => u.Username.ToLower() == username);
        if (exists)
            throw new Exception("Username already exists.");

        var user = new User
        {
            Username = dto.Username.Trim(),
            PasswordHash = HashPassword(dto.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponseDto { Token = CreateToken(user) };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var username = dto.Username.Trim().ToLower();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username);
        if (user == null)
            throw new Exception("Invalid username or password.");

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Invalid username or password.");

        return new AuthResponseDto { Token = CreateToken(user) };
    }

    
    // Password hashing (simple + safe enough for class project)
    
    private static string HashPassword(string password)
    {
        // generate random salt
        var salt = RandomNumberGenerator.GetBytes(16);

        // PBKDF2 hash
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );

        // store as: base64(salt).base64(hash)
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var storedHash = Convert.FromBase64String(parts[1]);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: 32
        );

        return CryptographicOperations.FixedTimeEquals(hash, storedHash);
    }

    
    // JWT token creation
    
    private string CreateToken(User user)
    {
        var key = _config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(key))
            throw new Exception("Jwt:Key missing in appsettings.json");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
