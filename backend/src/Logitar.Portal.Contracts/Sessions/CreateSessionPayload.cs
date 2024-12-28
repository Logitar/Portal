namespace Logitar.Portal.Contracts.Sessions;

public record CreateSessionPayload
{
  public Guid? Id { get; set; }

  public string User { get; set; }
  public bool IsPersistent { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public CreateSessionPayload() : this(string.Empty)
  {
  }

  public CreateSessionPayload(string user)
  {
    User = user;
    CustomAttributes = [];
  }

  public CreateSessionPayload(string user, bool isPersistent, IEnumerable<CustomAttribute> customAttributes) : this(user)
  {
    IsPersistent = isPersistent;
    CustomAttributes.AddRange(customAttributes);
  }
}
