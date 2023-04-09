namespace Logitar.Portal.v2.Contracts.Users.Contact;

public abstract record ContactInput
{
  public bool Verify { get; set; }
}
