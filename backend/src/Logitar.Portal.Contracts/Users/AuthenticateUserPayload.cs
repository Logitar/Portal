namespace Logitar.Portal.Contracts.Users;

public record AuthenticateUserPayload
{
  public string UniqueName { get; set; }
  public string Password { get; set; }

  public AuthenticateUserPayload() : this(string.Empty, string.Empty)
  {
  }

  public AuthenticateUserPayload(string uniqueName, string password)
  {
    UniqueName = uniqueName;
    Password = password;
  }
}
