using System.Security.Claims;

namespace Portal.Core.Tokens
{
  public interface ISecurityTokenService
  {
    string Create(ClaimsIdentity identity, string? algorithm = null, string? audience = null, DateTime? expires = null, string? issuer = null);
    Task<ValidateTokenResult> ValidateAsync(string token, string? audience = null, string? issuer = null, string? purpose = null, bool consume = false, CancellationToken cancellationToken = default);
  }
}
