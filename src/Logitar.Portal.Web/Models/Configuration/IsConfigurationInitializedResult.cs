namespace Logitar.Portal.Web.Models.Configuration;

public record IsConfigurationInitializedResult
{
  public IsConfigurationInitializedResult(Contracts.Configurations.Configuration? configuration)
  {
    IsInitialized = configuration != null;
  }

  public bool IsInitialized { get; }
}
