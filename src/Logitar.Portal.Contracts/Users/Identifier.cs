using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Contracts.Users;

public record Identifier
{
  public Guid Id { get; set; }

  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }

  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public long Version { get; set; }

  public string Key { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
}
