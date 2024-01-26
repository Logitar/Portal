using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record EmailPayload : IEmail
{
  public string Address { get; set; }
  public bool IsVerified { get; set; }

  public EmailPayload() : this(string.Empty)
  {
  }

  public EmailPayload(string address, bool isVerified = false)
  {
    Address = address;
    IsVerified = isVerified;
  }
}
