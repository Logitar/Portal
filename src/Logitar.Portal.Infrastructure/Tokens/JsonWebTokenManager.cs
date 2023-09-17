using Logitar.Portal.Application.Tokens;
using Microsoft.IdentityModel.Tokens;

namespace Logitar.Portal.Infrastructure.Tokens;

internal class JsonWebTokenManager : ITokenManager
{
  private const string DefaultAlgorithm = SecurityAlgorithms.HmacSha256;

  private readonly JwtSecurityTokenHandler _tokenHandler = new();

  public JsonWebTokenManager()
  {
    _tokenHandler.InboundClaimTypeMap.Clear();
  }

  public string Create(ClaimsIdentity subject, string secret, DateTime? expires, string? algorithm, string? audience, string? issuer, string? type)
  {
    SigningCredentials signingCredentials = new(GetSecurityKey(secret), algorithm ?? DefaultAlgorithm);

    SecurityTokenDescriptor tokenDescriptor = new()
    {
      Audience = audience,
      Expires = expires,
      Issuer = issuer,
      SigningCredentials = signingCredentials,
      Subject = subject,
      TokenType = type
    };

    SecurityToken token = _tokenHandler.CreateToken(tokenDescriptor);

    return _tokenHandler.WriteToken(token);
  }

  public ClaimsPrincipal Validate(string token, string secret, string? audience, string? issuer, string? type)
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
    if (type != null)
    {
      validationParameters.ValidTypes = new[] { type };
    }

    return _tokenHandler.ValidateToken(token, validationParameters, out _);
  }

  private static SecurityKey GetSecurityKey(string secret) => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
}
