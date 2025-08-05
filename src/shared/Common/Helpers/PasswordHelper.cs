// FILE: src/shared/Common/Helpers/PasswordHelper.cs
using BCrypt.Net;

namespace Common.Helpers;

public static class PasswordHelper
{
    private const int WorkFactor = 12;

    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    public static string GenerateRandomPassword(int length = 12)
    {
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string digitChars = "0123456789";
        const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        var random = new Random();
        var password = new List<char>();

        // Ensure at least one character from each category
        password.Add(upperChars[random.Next(upperChars.Length)]);
        password.Add(lowerChars[random.Next(lowerChars.Length)]);
        password.Add(digitChars[random.Next(digitChars.Length)]);
        password.Add(specialChars[random.Next(specialChars.Length)]);

        // Fill the rest randomly
        var allChars = upperChars + lowerChars + digitChars + specialChars;
        for (int i = 4; i < length; i++)
        {
            password.Add(allChars[random.Next(allChars.Length)]);
        }

        // Shuffle the password
        for (int i = password.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (password[i], password[j]) = (password[j], password[i]);
        }

        return new string(password.ToArray());
    }
}
