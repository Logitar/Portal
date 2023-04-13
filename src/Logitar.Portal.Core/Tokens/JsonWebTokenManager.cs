using Logitar.Portal.Core.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Logitar.Portal.Core.Tokens;

internal class JsonWebTokenManager : ITokenManager
{
  private const string DefaultAlgorithm = SecurityAlgorithms.HmacSha256;

  static JsonWebTokenManager()
  {
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
  }

  private readonly ITokenBlacklist _tokenBlacklist;
  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  public JsonWebTokenManager(ITokenBlacklist tokenBlacklist)
  {
    _tokenBlacklist = tokenBlacklist;
  }

  public string Create(ClaimsIdentity subject, string secret, string? algorithm, DateTime? expires, string? audience, string? issuer)
  {
    SigningCredentials signingCredentials = new(GetSecurityKey(secret), algorithm ?? DefaultAlgorithm);

    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = audience,
      Expires = expires,
      Issuer = issuer,
      SigningCredentials = signingCredentials,
      Subject = subject
    };

    SecurityToken token = _tokenHandler.CreateToken(tokenDescriptor);

    return _tokenHandler.WriteToken(token);
  }

  public async Task<ClaimsPrincipal> ValidateAsync(string token, string secret, string? audience, string? issuer, string? purpose, bool consume, CancellationToken cancellationToken)
  {
    SecurityKey key = GetSecurityKey(secret);

    TokenValidationParameters validationParameters = new()
    {
      IssuerSigningKey = key,
      ValidAudience = audience,
      ValidIssuer = issuer,
      ValidateAudience = audience != null,
      ValidateIssuer = issuer != null,
      ValidateIssuerSigningKey = true
    };

    ClaimsPrincipal principal = _tokenHandler.ValidateToken(token, validationParameters, out _);

    IEnumerable<Guid> ids = principal.FindAll(Rfc7519ClaimTypes.TokenId).Select(x => Guid.Parse(x.Value));
    if (ids.Any())
    {
      IEnumerable<Guid> blacklisted = await _tokenBlacklist.GetBlacklistedAsync(ids, cancellationToken);
      if (blacklisted.Any())
      {
        throw new SecurityTokenBlacklistedException(blacklisted);
      }
    }

    if (purpose != null)
    {
      IEnumerable<Claim> claims = principal.FindAll(Rfc7519ClaimTypes.Purpose);
      HashSet<string> purposes = claims.SelectMany(x => x.Value.Split().Select(y => y.ToLower())).ToHashSet();
      if (!purposes.Contains(purpose.ToLower()))
      {
        throw new InvalidSecurityTokenPurposeException(purpose, purposes);
      }
    }

    if (consume)
    {
      DateTime? expiresOn = principal.FindFirst(Rfc7519ClaimTypes.Expires)?.GetDateTime().Add(validationParameters.ClockSkew);

      await _tokenBlacklist.BlacklistAsync(ids, expiresOn, cancellationToken);
    }

    return principal;
  }

  private static SecurityKey GetSecurityKey(string secret) => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
}
