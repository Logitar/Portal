namespace Logitar.Portal.Infrastructure2.Entities
{
  internal class ConfigurationEntity
  {
    public ConfigurationEntity(string key, bool value) : this(key, value.ToString())
    {
    }
    public ConfigurationEntity(string key, int value) : this(key, value.ToString())
    {
    }
    public ConfigurationEntity(string key, string value)
    {
      Key = key;
      Value = value;
    }
    private ConfigurationEntity()
    {
    }

    public string Key { get; private set; } = null!;
    public string Value { get; private set; } = null!;
  }
}
