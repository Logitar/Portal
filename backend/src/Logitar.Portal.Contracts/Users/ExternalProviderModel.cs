using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Contracts.Users
{
  public record ExternalProviderModel
  {
    public Guid Id { get; set; }

    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? DisplayName { get; set; }

    public ActorModel? AddedBy { get; set; }
    public DateTime AddedOn { get; set; }
  }
}
