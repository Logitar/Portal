using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Tokens;

public record ValidatedToken
{
  public string? Subject { get; set; }
  public Email? Email { get; set; }
  public List<TokenClaim> Claims { get; set; } = [];
}
