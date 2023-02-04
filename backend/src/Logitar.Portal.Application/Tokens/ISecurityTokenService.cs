using System.Security.Claims;

namespace Logitar.Portal.Application.Tokens
{
  public interface ISecurityTokenService
  {
    string Create(ClaimsIdentity identity, string secret, string? audience = null, DateTime? expires = null, string? issuer = null);
    string Create(ClaimsIdentity identity, string secret, string? algorithm = null, string? audience = null, DateTime? expires = null, string? issuer = null);
    Task<ValidateTokenResult> ValidateAsync(string token, string secret, string? audience = null, string? issuer = null, string? purpose = null, bool consume = false, CancellationToken cancellationToken = default);
  }
}
