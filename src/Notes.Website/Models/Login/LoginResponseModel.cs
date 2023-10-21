namespace Notes.Website.Models.Login;

public class LoginResponseModel
{
    public required string access_token { get; set; }
    public required long expires_in { get; set; }
    public string token_type => "Bearer";
}