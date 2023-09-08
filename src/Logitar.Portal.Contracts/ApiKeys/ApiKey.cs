using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Contracts.ApiKeys;

public record ApiKey : Aggregate
{
  public Guid Id { get; set; }

  public string? XApiKey { get; set; }

  public string DisplayName { get; set; } = string.Empty;
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public IEnumerable<CustomAttribute> CustomAttributes { get; set; } = Enumerable.Empty<CustomAttribute>();

  public IEnumerable<Role> Roles { get; set; } = Enumerable.Empty<Role>();

  public Realm? Realm { get; set; }
}
