namespace Logitar.Portal.Contracts.Roles;

public record RoleModification
{
  public RoleModification() : this(string.Empty)
  {
  }
  public RoleModification(string role, CollectionAction action = CollectionAction.Add)
  {
    Role = role;
    Action = action;
  }

  public string Role { get; set; }
  public CollectionAction Action { get; set; }
}
