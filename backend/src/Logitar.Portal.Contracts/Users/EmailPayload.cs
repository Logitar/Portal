using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record EmailPayload : IEmail
{
  public string Address { get; set; }

  public EmailPayload() : this(string.Empty)
  {
  }

  public EmailPayload(string address)
  {
    Address = address;
  }
}
