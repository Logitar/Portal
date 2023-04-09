using Logitar.Portal.v2.Contracts.Actors;

namespace Logitar.Portal.v2.Contracts.Users;

public record ExternalIdentifier
{
  public Guid Id { get; set; }

  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }
}
