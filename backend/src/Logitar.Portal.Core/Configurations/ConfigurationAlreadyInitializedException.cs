using System.Net;

namespace Logitar.Portal.Core.Configurations
{
  internal class ConfigurationAlreadyInitializedException : ApiException
  {
    public ConfigurationAlreadyInitializedException()
      : base(HttpStatusCode.Forbidden, "The configuration has already been initialized.")
    {
    }
  }
}
