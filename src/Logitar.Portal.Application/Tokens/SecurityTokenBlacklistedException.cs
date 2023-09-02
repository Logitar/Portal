using Microsoft.IdentityModel.Tokens;

namespace Logitar.Portal.Application.Tokens;

public class SecurityTokenBlacklistedException : SecurityTokenValidationException
{
  private const string ErrorMessage = "The security token is blacklisted.";

  public SecurityTokenBlacklistedException(IEnumerable<Guid> blacklistedIds)
    : base(BuildMessage(blacklistedIds))
  {
    BlacklistedIds = blacklistedIds;
  }

  public IEnumerable<Guid> BlacklistedIds
  {
    get => (IEnumerable<Guid>)Data[nameof(BlacklistedIds)]!;
    private set => Data[nameof(BlacklistedIds)] = value;
  }

  private static string BuildMessage(IEnumerable<Guid> blacklistedIds)
  {
    StringBuilder message = new();

    message.AppendLine(ErrorMessage);
    message.AppendLine("BlacklistedIds:");
    foreach (Guid blacklistedId in blacklistedIds)
    {
      message.Append(" - ").Append(blacklistedId).AppendLine();
    }

    return message.ToString();
  }
}
