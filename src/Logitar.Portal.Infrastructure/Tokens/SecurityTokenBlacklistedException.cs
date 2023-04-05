using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Logitar.Portal.Infrastructure.Tokens
{
  internal class SecurityTokenBlacklistedException : SecurityTokenValidationException
  {
    public SecurityTokenBlacklistedException(IEnumerable<Guid> ids) : base(GetMessage(ids))
    {
      Ids = ids ?? throw new ArgumentNullException(nameof(ids));
    }

    public IEnumerable<Guid> Ids { get; }

    private static string GetMessage(IEnumerable<Guid> ids)
    {
      var message = new StringBuilder();

      message.AppendLine("The security token is blacklisted.");
      message.AppendLine($"Blacklisted IDs: {string.Join(", ", ids)}");

      return message.ToString();
    }
  }
}
