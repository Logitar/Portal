using System.Collections.Generic;

namespace Logitar.Portal.Contracts.Tokens
{
  public record CreateTokenPayload
  {
    public int? Lifetime { get; set; }
    public string? Purpose { get; set; }
    public string? Realm { get; set; }

    public string? Email { get; set; }
    public string? Subject { get; set; }

    public IEnumerable<ClaimPayload>? Claims { get; set; }
  }
}
