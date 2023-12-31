namespace ContactsAPI.Config;

public class MongoSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string ContactCollection { get; set; } = null!;
    public string AccountCollection { get; set; } = null!;
    public string CredentialsCollection { get; set; } = null!;
}
