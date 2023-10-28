namespace Notes.Website.Models.Login
{
    public class LoginRequestModel
    {
        public string? UserName { get; set; } = null;
        public string? Password { get; set; } = null;
        public bool RememberMe { get; set; } = false;
        public string? ReturnUrl { get; set; } = null;
    }
}
