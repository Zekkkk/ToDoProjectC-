using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ToDo.Api.Domain.Entities;

namespace ToDo.Api.Common.Auth;

// USER NEED: Stay logged in after login
// DEV: Generate JWT token with UserId inside
public class JwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(JwtSettings settings)
    {
        _settings = settings;
    }

    public string GenerateToken(User user)
    {
        // Create claims (data inside token)
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        // Convert secret to bytes
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.SecretKey));

        // Create signature
        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // Create token object
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            signingCredentials: creds
        );

        // Convert token to string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}