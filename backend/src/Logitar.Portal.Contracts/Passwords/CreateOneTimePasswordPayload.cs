namespace Logitar.Portal.Contracts.Passwords;

public class CreateOneTimePasswordPayload
{
  public string Characters { get; set; }
  public int Length { get; set; }

  public DateTime? ExpiresOn { get; set; }
  public int? MaximumAttempts { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; }

  public CreateOneTimePasswordPayload() : this(string.Empty, 0)
  {
  }

  public CreateOneTimePasswordPayload(string characters, int length)
  {
    Characters = characters;
    Length = length;
    CustomAttributes = [];
  }
}
