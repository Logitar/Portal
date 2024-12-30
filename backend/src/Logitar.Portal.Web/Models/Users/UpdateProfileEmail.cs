using Logitar.Identity.Contracts.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Web.Models.Users;

public record UpdateProfileEmail : IEmail
{
  public string Address { get; set; } = string.Empty;
  public bool IsVerified { get; set; }

  public EmailPayload ToPayload() => new(Address, isVerified: false);
}
