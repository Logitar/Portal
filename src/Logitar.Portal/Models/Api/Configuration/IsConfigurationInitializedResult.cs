namespace Logitar.Portal.Models.Api.Configuration;

public record IsConfigurationInitializedResult
{
  public IsConfigurationInitializedResult(Contracts.Configurations.Configuration? configuration)
  {
    IsInitialized = configuration != null;
  }

  public bool IsInitialized { get; }
}
