using System.Configuration;

namespace Notes.Business.Configurations
{
    public class NotesConfig
    {
        public DatabaseConfig Database { get; set; } = new();
        public IdentityConfig Identity { get; set; } = new();
        public LoginConfig Login { get; set; } = new();
        public SigningConfig Signing { get; set; } = new();

        public string Urls { get; set; } = default!;

        public void Validate()
        {
            Database.Validate();
            Identity.Validate();
            Login.Validate();
            Signing.Validate();

            if (string.IsNullOrEmpty(Urls))
            {
                throw new ConfigurationErrorsException("Urls is a Required Configuration");
            }
        }
    }
}
