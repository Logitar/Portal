using Microsoft.IdentityModel.Tokens;
using Portal.Core;
using Portal.Core.Claims;
using Portal.Core.Tokens;
using Portal.Infrastructure.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Portal.Infrastructure.Tokens
{
  internal class JwtService : ISecurityTokenService
  {
    private const string DefaultAlgorithm = SecurityAlgorithms.HmacSha256;

    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly IJwtBlacklist _blacklist;
    private readonly SecurityKey _key;

    public JwtService(IJwtBlacklist blacklist, JwtSettings settings)
    {
      ArgumentNullException.ThrowIfNull(blacklist);
      ArgumentNullException.ThrowIfNull(settings);

      if (settings.Secret == null)
      {
        throw new ArgumentException($"The {nameof(settings.Secret)} is required.", nameof(settings));
      }

      _blacklist = blacklist;
      _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.Secret));
    }

    public string Create(ClaimsIdentity subject, string? algorithm, string? audience, DateTime? expires, string? issuer)
    {
      ArgumentNullException.ThrowIfNull(subject);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Audience = audience,
        Expires = expires,
        Issuer = issuer,
        SigningCredentials = new SigningCredentials(_key, algorithm ?? DefaultAlgorithm),
        Subject = subject
      };

      SecurityToken token = _tokenHandler.CreateToken(tokenDescriptor);

      return _tokenHandler.WriteToken(token);
    }

    public async Task<ValidateTokenResult> ValidateAsync(string token, string? audience, string? issuer, string? purpose, bool consume, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(token);

      var validationParameters = new TokenValidationParameters
      {
        IssuerSigningKey = _key,
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
          DateTime? expiresAt = principal.FindFirst(Rfc7519ClaimTypes.Expires)?.GetDateTime();
          await _blacklist.BlacklistAsync(ids, expiresAt, cancellationToken);
        }

        return ValidateTokenResult.Success(principal);
      }
      catch (SecurityTokenException exception)
      {
        return ValidateTokenResult.Failed(new Error(
          code: exception.GetType().Name.Replace("Exception", string.Empty),
          description: exception.Message
        ));
      }
    }
  }
}
