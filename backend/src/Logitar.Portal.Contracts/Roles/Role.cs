using Logitar.Portal.Contracts.Realms;

namespace Logitar.Portal.Contracts.Roles;

public class Role : Aggregate
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public RealmModel? Realm { get; set; }

  public Role() : this(string.Empty)
  {
  }

  public Role(string uniqueName)
  {
    UniqueName = uniqueName;
    CustomAttributes = [];
  }
}
