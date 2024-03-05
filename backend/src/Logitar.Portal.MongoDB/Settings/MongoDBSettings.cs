namespace Logitar.Portal.MongoDB.Settings;

public record MongoDBSettings
{
  public string ConnectionString { get; set; }
  public string DatabaseName { get; set; }

  public MongoDBSettings() : this(string.Empty, string.Empty)
  {
  }

  public MongoDBSettings(string connectionString, string databaseName)
  {
    ConnectionString = connectionString;
    DatabaseName = databaseName;
  }
}
