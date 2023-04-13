namespace Logitar.Portal.Contracts.Users.Contact;

public record EmailInput : ContactInput
{
  public string Address { get; set; } = string.Empty;
}
