using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record Email : Contact, IEmail
{
  public string Address { get; set; }

  public Email() : this(string.Empty)
  {
  }

  public Email(string address)
  {
    Address = address;
  }
}
