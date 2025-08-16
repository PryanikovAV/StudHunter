namespace StudHunter.API.Services;

/// <summary>
/// Interface for password hashing.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hash to verify against.</param>
    /// <returns>True if the password is valid, false otherwise.</returns>
    bool VerifyPassword(string password, string hashedPassword);
}

/// <summary>
/// Implementation of IPasswordHasher using BCrypt.
/// </summary>
public class PasswordHasher : IPasswordHasher
{

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }
}
