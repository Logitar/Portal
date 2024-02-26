namespace Logitar.Portal.Web.Models.Configuration;

public record IsConfigurationInitialized
{
  public bool IsInitialized { get; set; }

  public IsConfigurationInitialized() : this(isInitialized: false)
  {
  }

  public IsConfigurationInitialized(Contracts.Configurations.Configuration? configuration) : this(isInitialized: configuration != null)
  {
  }

  public IsConfigurationInitialized(bool isInitialized)
  {
    IsInitialized = isInitialized;
  }
}
