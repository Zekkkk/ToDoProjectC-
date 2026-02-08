using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using ToDo.Api.Common.Auth;
using ToDo.Api.DTO.Auth;
using ToDo.Api.Infrastructure.Data;
using ToDo.Api.Services.Interfaces;
using ToDo.Api.Domain.Entities;

namespace ToDo.Api.Services;

/// <summary>
/// USER NEED: Register and login to get a JWT token for protected endpoints.
/// DEV: Service validates users and delegates token creation to JwtTokenGenerator.
/// WHY REPO/DTO: DTOs define inputs, while repositories are used for task/subtask CRUD.
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtTokenGenerator _tokenGenerator;

    public AuthService(AppDbContext db, JwtTokenGenerator tokenGenerator)
    {
        _db = db;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var username = dto.Username.Trim().ToLower();

        var exists = await _db.Users.AnyAsync(u => u.Username.ToLower() == username);
        if (exists)
            throw new InvalidOperationException("Username already exists.");

        var user = new User
        {
            Username = dto.Username.Trim(),
            PasswordHash = HashPassword(dto.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponseDto { Token = _tokenGenerator.GenerateToken(user) };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var username = dto.Username.Trim().ToLower();

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password.");

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password.");

        return new AuthResponseDto { Token = _tokenGenerator.GenerateToken(user) };
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

}
