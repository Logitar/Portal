using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Roles;

public class RoleModel : AggregateModel
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public RealmModel? Realm { get; set; }

  public RoleModel() : this(string.Empty)
  {
  }

  public RoleModel(string uniqueName)
  {
    UniqueName = uniqueName;
    CustomAttributes = [];
  }
}
