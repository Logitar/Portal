namespace Logitar.Portal.Application.Configurations;

public class ConfigurationAlreadyInitializedException : Exception
{
  public const string ErrorMessage = "The configuration has already been initialized.";

  public ConfigurationAlreadyInitializedException() : base(ErrorMessage)
  {
  }
}
