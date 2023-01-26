namespace Logitar.Portal.Core2.Configurations
{
  internal class ConfigurationAlreadyInitializedException : Exception
  {
    public ConfigurationAlreadyInitializedException() : base("The configuration has already been initialized.")
    {
    }
  }
}
