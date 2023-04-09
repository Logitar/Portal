namespace Logitar.Portal.v2.Contracts.Users.Contact;

public record Email : Contact
{
  public string Address { get; set; } = string.Empty;
}
