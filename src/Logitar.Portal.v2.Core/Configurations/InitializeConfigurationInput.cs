namespace Logitar.Portal.v2.Core.Configurations;

public record InitializeConfigurationInput
{
  public InitialUserInput User { get; set; } = new();
}
