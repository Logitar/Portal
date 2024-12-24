using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models.Users;

public record UpdateProfileEmail : IEmail
{
  public string Address { get; set; }
  public bool IsVerified { get; set; }

  public UpdateProfileEmail() : this(string.Empty)
  {
  }

  public UpdateProfileEmail(string address)
  {
    Address = address;
  }

  public EmailPayload ToPayload() => new(Address, isVerified: false);
}
