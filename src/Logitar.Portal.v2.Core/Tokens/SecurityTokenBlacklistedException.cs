using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Logitar.Portal.v2.Core.Tokens;

public class SecurityTokenBlacklistedException : SecurityTokenValidationException
{
  public SecurityTokenBlacklistedException(IEnumerable<Guid> blacklistedIds) : base(GetMessage(blacklistedIds))
  {
    Data["BlacklistedIds"] = blacklistedIds;
  }

  private static string GetMessage(IEnumerable<Guid> blacklistedIds)
  {
    StringBuilder message = new();

    message.AppendLine("The security token is blacklisted.");
    message.AppendLine("Blacklisted identifiers:");
    foreach (Guid blacklistedId in blacklistedIds)
    {
      message.Append(" - ").Append(blacklistedId).AppendLine();
    }

    return message.ToString();
  }
}
