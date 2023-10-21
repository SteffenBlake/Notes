using System.Configuration;

namespace Notes.Business.Configurations;

public class DatabaseConfig
{
    public string ConnectionString { get; set; } = default!;

    public void Validate()
    {
        if (string.IsNullOrEmpty(ConnectionString))
        {
            throw new ConfigurationErrorsException("Database.ConnectionString is a Required Configuration");
        }
    }
}