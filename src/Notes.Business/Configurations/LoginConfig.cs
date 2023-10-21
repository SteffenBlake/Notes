using System.Configuration;

namespace Notes.Business.Configurations;

public class LoginConfig
{
    public string Email { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public TimeSpan TokenExpiry { get; set; } = TimeSpan.FromHours(1);

    public void Validate()
    {
        if (string.IsNullOrEmpty(Email))
        {
            throw new ConfigurationErrorsException("Login.Email is a Required Configuration");
        }
        if (string.IsNullOrEmpty(UserName))
        {
            throw new ConfigurationErrorsException("Login.UserName is a Required Configuration");
        }
        if (string.IsNullOrEmpty(Password))
        {
            throw new ConfigurationErrorsException("Login.Password is a Required Configuration");
        }
    }
}