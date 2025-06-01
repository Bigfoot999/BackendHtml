using System;
using System.Security.Cryptography;
using System.Text;

public class PasswordHasher
{
    public static string HashPassword(string password)
    {
        using SHA256 sha256Hash = SHA256.Create();
        // Convert the input string to a byte array and compute the hash.
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

        // Convert the byte array to a hexadecimal string.
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }

    public static bool VerifyPassword(string inputPassword, string storedHash)
    {
        // Hash the input password.
        string hashOfInput = HashPassword(inputPassword);

        // Compare the hashed input password with the stored hash.
        return hashOfInput.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
    }
}