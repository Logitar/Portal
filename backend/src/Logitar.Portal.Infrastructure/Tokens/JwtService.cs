using Logitar.Portal.Application;
using Logitar.Portal.Application.Claims;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Logitar.Portal.Infrastructure.Tokens
{
  internal class JwtService : ISecurityTokenService
  {
    private const string DefaultAlgorithm = SecurityAlgorithms.HmacSha256;

    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly IJwtBlacklist _blacklist;
    private readonly ICacheService _cacheService;

    public JwtService(IJwtBlacklist blacklist, ICacheService cacheService)
    {
      _blacklist = blacklist;
      _cacheService = cacheService;
    }

    public string Create(ClaimsIdentity subject, string? secret, string? audience, DateTime? expires, string? issuer)
      => Create(subject, secret, algorithm: null, audience, expires, issuer);
    public string Create(ClaimsIdentity subject, string? secret, string? algorithm, string? audience, DateTime? expires, string? issuer)
    {
      secret ??= _cacheService.Configuration?.JwtSecret
        ?? throw new InvalidOperationException("The JWT secret could not be resolved.");

      SecurityTokenDescriptor tokenDescriptor = new()
      {
        Audience = audience,
        Expires = expires,
        Issuer = issuer,
        SigningCredentials = new SigningCredentials(GetSecurityKey(secret), algorithm ?? DefaultAlgorithm),
        Subject = subject
      };

      SecurityToken token = _tokenHandler.CreateToken(tokenDescriptor);

      return _tokenHandler.WriteToken(token);
    }

    public async Task<ValidateTokenResult> ValidateAsync(string token, string? secret, string? audience, string? issuer, string? purpose, bool consume, CancellationToken cancellationToken)
    {
      secret ??= _cacheService.Configuration?.JwtSecret
        ?? throw new InvalidOperationException("The JWT secret could not be resolved.");

      TokenValidationParameters validationParameters = new()
      {
        IssuerSigningKey = GetSecurityKey(secret),
        ValidAudience = audience,
        ValidIssuer = issuer,
        ValidateAudience = audience != null,
        ValidateIssuer = issuer != null,
        ValidateIssuerSigningKey = true
      };

      try
      {
        ClaimsPrincipal principal = _tokenHandler.ValidateToken(token, validationParameters, out _);

        IEnumerable<Guid> ids = principal.FindAll(Rfc7519ClaimTypes.JwtId).Select(x => Guid.Parse(x.Value));
        if (ids.Any())
        {
          IEnumerable<Guid> blacklisted = await _blacklist.GetBlacklistedAsync(ids, cancellationToken);
          if (blacklisted.Any())
          {
            throw new SecurityTokenBlacklistedException(blacklisted);
          }
        }

        if (purpose != null)
        {
          IEnumerable<Claim> claims = principal.FindAll(CustomClaimTypes.Purpose);
          HashSet<string> purposes = claims.SelectMany(x => x.Value.Split().Select(y => y.ToLower())).ToHashSet();
          if (!purposes.Contains(purpose.ToLower()))
          {
            throw new InvalidSecurityTokenPurposeException(purpose, purposes);
          }
        }

        if (consume)
        {
          DateTime? expiresOn = principal.FindFirst(Rfc7519ClaimTypes.Expires)
            ?.GetDateTime()
            .Add(validationParameters.ClockSkew);

          await _blacklist.BlacklistAsync(ids, expiresOn, cancellationToken);
        }

        return ValidateTokenResult.Success(principal);
      }
      catch (Exception exception)
      {
        return ValidateTokenResult.Failed(new Error(exception));
      }
    }

    private static SecurityKey GetSecurityKey(string secret) => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
  }
}
