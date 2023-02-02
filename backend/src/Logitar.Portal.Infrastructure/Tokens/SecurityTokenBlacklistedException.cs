using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Logitar.Portal.Infrastructure.Tokens
{
  internal class SecurityTokenBlacklistedException : SecurityTokenValidationException
  {
    public SecurityTokenBlacklistedException(IEnumerable<Guid> ids) : base(GetMessage(ids))
    {
      Data["Ids"] = ids;
    }

    private static string GetMessage(IEnumerable<Guid> ids)
    {
      StringBuilder message = new();

      message.AppendLine("The security token is blacklisted.");
      message.AppendLine($"Blacklisted IDs: {string.Join(", ", ids)}");

      return message.ToString();
    }
  }
}
