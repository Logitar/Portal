namespace Logitar.Portal.Contracts.Sessions;

public record SignInSessionPayload
{
  public string? Id { get; set; }

  public string UniqueName { get; set; }
  public string Password { get; set; }
  public bool IsPersistent { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public SignInSessionPayload() : this(string.Empty, string.Empty)
  {
  }

  public SignInSessionPayload(string uniqueName, string password, bool isPersistent = false)
  {
    UniqueName = uniqueName;
    Password = password;
    IsPersistent = isPersistent;
    CustomAttributes = [];
  }
}
