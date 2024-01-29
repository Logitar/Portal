using Logitar.Identity.Contracts.Users;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users;

internal static class ContactExtensions
{
  public static EmailUnit ToEmailUnit(this EmailPayload email) => email.ToEmailUnit(email.IsVerified);
  public static EmailUnit ToEmailUnit(this IEmail email, bool isVerified = false)
  {
    return new EmailUnit(email.Address, isVerified);
  }
}
