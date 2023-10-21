using Microsoft.AspNetCore.Identity;

namespace Notes.Business.Configurations;

public class IdentityConfig
{
    public LockoutOptions Lockout { get; set; } = new ();
    public PasswordOptions Password { get; set; } = new();
    public SignInOptions SignIn { get; set; } = new();
    public UserOptions User { get; set; } = new();

    public void Validate()
    {
    }
}