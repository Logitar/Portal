namespace Logitar.Portal.Contracts.Sessions;

public record SignInSessionPayload
{
  public string UniqueName { get; set; }
  public string Password { get; set; }
  public bool IsPersistent { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public SignInSessionPayload() : this(string.Empty, string.Empty)
  {
  }

  public SignInSessionPayload(string uniqueName, string password)
  {
    UniqueName = uniqueName;
    Password = password;
    CustomAttributes = [];
  }

  public SignInSessionPayload(string uniqueName, string password, bool isPersistent, IEnumerable<CustomAttribute> customAttributes)
    : this(uniqueName, password)
  {
    IsPersistent = isPersistent;
    CustomAttributes.AddRange(customAttributes);
  }
}
