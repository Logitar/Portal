using Logitar.Portal.v2.Contracts.Errors;

namespace Logitar.Portal.v2.Contracts.Tokens;

public record ValidatedToken
{
  public IEnumerable<Error> Errors { get; set; } = Enumerable.Empty<Error>();
  public bool Succeeded => !Errors.Any();

  public string? Subject { get; set; }
  public string? EmailAddress { get; set; }
  public IEnumerable<TokenClaim>? Claims { get; set; }
}
