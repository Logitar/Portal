namespace Logitar.Portal.Core.Configurations
{
  internal class ConfigurationAlreadyInitializedException : Exception
  {
    public ConfigurationAlreadyInitializedException() : base("The configuration has already been initialized.")
    {
    }
  }
}
