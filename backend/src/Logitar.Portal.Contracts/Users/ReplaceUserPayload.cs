namespace Logitar.Portal.Contracts.Users;

public record ReplaceUserPayload
{
  public string UniqueName { get; set; }
  public string? Password { get; set; }
  public bool IsDisabled { get; set; }

  public ReplaceUserPayload() : this(string.Empty)
  {
  }

  public ReplaceUserPayload(string uniqueName, string? password = null)
  {
    UniqueName = uniqueName;
    Password = password;
  }
}
