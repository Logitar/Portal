using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Domain.Users;

public record EmailAddress : IEmailAddress
{
  public EmailAddress(string address, bool isVerified = false)
  {
    Address = address.Trim();
    IsVerified = isVerified;
  }

  public string Address { get; }
  public bool IsVerified { get; }
}
