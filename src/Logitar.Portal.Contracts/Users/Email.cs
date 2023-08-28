namespace Logitar.Portal.Contracts.Users;

public record Email : Contact, IEmailAddress
{
  public string Address { get; set; } = string.Empty;
}
