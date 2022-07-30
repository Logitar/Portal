using FluentValidation;
using Portal.Core.Claims;
using Portal.Core.Realms;
using Portal.Core.Tokens.Models;
using Portal.Core.Tokens.Payloads;
using System.Security.Claims;

namespace Portal.Core.Tokens
{
  internal class TokenService : ITokenService
  {
    private readonly IRealmQuerier _realmQuerier;
    private readonly ISecurityTokenService _securityTokenService;
    private readonly IUserContext _userContext;
    private readonly IValidator<CreateTokenPayload> _validator;

    public TokenService(
      IRealmQuerier realmQuerier,
      ISecurityTokenService securityTokenService,
      IUserContext userContext,
      IValidator<CreateTokenPayload> validator
    )
    {
      _realmQuerier = realmQuerier;
      _securityTokenService = securityTokenService;
      _userContext = userContext;
      _validator = validator;
    }

    public async Task<TokenModel> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      _validator.ValidateAndThrow(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = (Guid.TryParse(payload.Realm, out Guid guid)
          ? await _realmQuerier.GetAsync(guid, readOnly: true, cancellationToken)
          : await _realmQuerier.GetAsync(alias: payload.Realm, readOnly: true, cancellationToken)
        ) ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      var identity = new ClaimsIdentity();
      identity.AddClaim(new(Rfc7519ClaimTypes.JwtId, Guid.NewGuid().ToString()));

      var expires = payload.Lifetime.HasValue
        ? DateTime.UtcNow.AddSeconds(payload.Lifetime.Value)
        : (DateTime?)null;

      if (payload.Purpose != null)
        identity.AddClaim(new(CustomClaimTypes.Purpose, payload.Purpose.ToLower()));

      if (payload.Email != null)
        identity.AddClaim(new(Rfc7519ClaimTypes.Email, payload.Email));
      if (payload.Subject != null)
        identity.AddClaim(new(Rfc7519ClaimTypes.Subject, payload.Subject));

      if (payload.Claims != null)
        identity.AddClaims(payload.Claims.Select(claim => new Claim(claim.Type, claim.Value, claim.ValueType)));

      string token = _securityTokenService.Create(identity, algorithm: null, GetAudience(realm), expires, GetIssuer(realm));

      return new TokenModel(token);
    }

    public async Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, bool consume, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = (Guid.TryParse(payload.Realm, out Guid guid)
          ? await _realmQuerier.GetAsync(guid, readOnly: true, cancellationToken)
          : await _realmQuerier.GetAsync(alias: payload.Realm, readOnly: true, cancellationToken)
        ) ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      ValidateTokenResult result = await _securityTokenService
        .ValidateAsync(payload.Token, GetAudience(realm), GetIssuer(realm), payload.Purpose, consume, cancellationToken);

      return new ValidatedTokenModel(result);
    }

    private string GetAudience(Realm? realm) => (realm?.Url ?? realm?.Alias ?? _userContext.BaseUrl).ToLower();
    private string GetIssuer(Realm? realm) => (realm == null ? _userContext.BaseUrl : $"{_userContext.BaseUrl}/realms/{realm.Alias}").ToLower();
  }
}
