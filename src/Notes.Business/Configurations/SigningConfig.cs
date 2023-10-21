using System.Configuration;

namespace Notes.Business.Configurations;

public class SigningConfig
{
    public string JWTSecret { get; set; } = default!;
    public void Validate()
    {
        if (string.IsNullOrEmpty(JWTSecret))
        {
            throw new ConfigurationErrorsException("Signing.JWTSecret is a Required Configuration");
        }

        if (JWTSecret.Length < 16)
        {
            throw new ConfigurationErrorsException("Signing.JWTSecret must be at least 16 characters long");
        }
    }
}