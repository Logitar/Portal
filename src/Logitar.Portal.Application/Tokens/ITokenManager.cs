using System.Security.Claims;

namespace Logitar.Portal.Application.Tokens;

public interface ITokenManager
{
  string Create(ClaimsIdentity identity, string secret, DateTime? expires = null, string? algorithm = null, string? audience = null, string? issuer = null);
}
