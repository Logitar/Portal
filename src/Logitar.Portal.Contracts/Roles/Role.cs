using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Roles;

public record Role : Aggregate
{
  public Guid Id { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();

  public Realm? Realm { get; set; }
}
