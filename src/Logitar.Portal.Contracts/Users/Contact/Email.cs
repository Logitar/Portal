namespace Logitar.Portal.Contracts.Users.Contact;

public record Email : Contact
{
  public string Address { get; set; } = string.Empty;
}
