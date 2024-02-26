using Logitar.Identity.Contracts.Users;

namespace Logitar.Portal.Contracts.Users;

public record EmailPayload : ContactPayload, IEmail
{
  public string Address { get; set; }

  public EmailPayload() : this(string.Empty, isVerified: false)
  {
  }

  public EmailPayload(string address, bool isVerified) : base(isVerified)
  {
    Address = address;
  }
}
