namespace ToDo.Api.Common.Auth;

// USER NEED: Secure login system
// DEV: Store JWT configuration in one place
public class JwtSettings
{
    // Secret key used to sign tokens
    public string SecretKey { get; set; } = string.Empty;

    // Who created the token
    public string Issuer { get; set; } = string.Empty;

    // Who can use the token
    public string Audience { get; set; } = string.Empty;

    // Token lifetime in minutes
    public int ExpiryMinutes { get; set; }
}