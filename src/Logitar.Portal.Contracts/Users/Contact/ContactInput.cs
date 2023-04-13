namespace Logitar.Portal.Contracts.Users.Contact;

public abstract record ContactInput
{
  public bool Verify { get; set; }
}
