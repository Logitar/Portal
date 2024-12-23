using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Contracts.ApiKeys;

public class ApiKeyModel : Aggregate
{
  public string? XApiKey { get; set; }

  public string DisplayName { get; set; }
  public string? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public List<RoleModel> Roles { get; set; }

  public RealmModel? Realm { get; set; }

  public ApiKeyModel() : this(string.Empty)
  {
  }

  public ApiKeyModel(string displayName)
  {
    DisplayName = displayName;
    CustomAttributes = [];
    Roles = [];
  }
}
