namespace Logitar.Portal.Contracts.Passwords;

public record ValidateOneTimePasswordPayload
{
  public string Password { get; set; }
  public List<CustomAttribute> CustomAttributes { get; set; }

  public ValidateOneTimePasswordPayload() : this(string.Empty)
  {
  }

  public ValidateOneTimePasswordPayload(string password)
  {
    Password = password;
    CustomAttributes = [];
  }
}
