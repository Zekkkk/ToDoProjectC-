using System.Security.Cryptography;
using System.Text;

namespace ToDo.Api.Common.Auth;

// USER NEED: Password must be stored safely
// DEV: Hash password instead of saving plain text
public static class PasswordHasher
{
    // Hash a password using SHA256
    public static string Hash(string password)
    {
        using var sha = SHA256.Create();

        var bytes = Encoding.UTF8.GetBytes(password);

        var hash = sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }

    // Compare plain password with stored hash
    public static bool Verify(string password, string hash)
    {
        return Hash(password) == hash;
    }
}