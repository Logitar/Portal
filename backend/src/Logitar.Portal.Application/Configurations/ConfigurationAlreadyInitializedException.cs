using Logitar.Portal.Contracts.Errors;

namespace Logitar.Portal.Application.Configurations;

public class ConfigurationAlreadyInitializedException : Exception, IErrorException
{
  public Error Error => new(this.GetErrorCode(), Message);

  public ConfigurationAlreadyInitializedException() : base("The configuration has already been initialized.")
  {
  }
}
