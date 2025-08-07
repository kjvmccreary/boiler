// FILE: src/services/AuthService/Services/PasswordService.cs
using BCrypt.Net;

namespace AuthService.Services;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    bool IsPasswordValid(string password);
}

public class PasswordService : IPasswordService
{
    private readonly ILogger<PasswordService> _logger;

    public PasswordService(ILogger<PasswordService> logger)
    {
        _logger = logger;
    }

    public string HashPassword(string password)
    {
        // Add input validation to match test expectations
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        try
        {
            // Correct BCrypt API usage
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hashing password");
            throw;
        }
    }

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            // Handle invalid inputs gracefully - return false instead of throwing
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            // Correct BCrypt API usage
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error verifying password");
            return false;
        }
    }

    public bool IsPasswordValid(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < 8)
            return false;

        // Add additional password complexity rules as needed
        return true;
    }

    // Quick test method - add this temporarily
    public void TestBCryptHash()
    {
        var testPassword = "password";
        var seedHash = "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LeKMVTcOoqc8WITq2";

        bool result = BCrypt.Net.BCrypt.Verify(testPassword, seedHash);
        Console.WriteLine($"BCrypt verification result for 'password': {result}");

        // Test what the actual hash should be
        var newHash = BCrypt.Net.BCrypt.HashPassword("password", 12);
        Console.WriteLine($"New hash for 'password': {newHash}");
    }
}
