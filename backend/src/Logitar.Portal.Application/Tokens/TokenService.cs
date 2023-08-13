using FluentValidation;
using Logitar.Portal.Application.Claims;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Tokens;
using Logitar.Portal.Core.Tokens.Models;
using Logitar.Portal.Core.Tokens.Payloads;
using Logitar.Portal.Domain.Realms;
using System.Security.Claims;

namespace Logitar.Portal.Application.Tokens
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
        realm = await _realmQuerier.GetAsync(payload.Realm, readOnly: true, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
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

      return new TokenModel
      {
        Token = token
      };
    }

    public async Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, bool consume, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _realmQuerier.GetAsync(payload.Realm, readOnly: true, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      ValidateTokenResult result = await _securityTokenService
        .ValidateAsync(payload.Token, GetAudience(realm), GetIssuer(realm), payload.Purpose, consume, cancellationToken);

      var model = new ValidatedTokenModel();
      if (result.Principal == null)
      {
        model.Errors = result.Errors.Select(error => new ErrorModel
        {
          Code = error.Code,
          Description = error.Description,
          Data = error.Data?.Select(x => new ErrorDataModel
          {
            Key = x.Key,
            Value = x.Value
          }) ?? Enumerable.Empty<ErrorDataModel>()
        });
      }
      else
      {
        var claims = new List<ClaimModel>(capacity: result.Principal.Claims.Count());

        foreach (Claim claim in result.Principal.Claims)
        {
          switch (claim.Type)
          {
            case Rfc7519ClaimTypes.Email:
              model.Email = claim.Value;
              break;
            case Rfc7519ClaimTypes.Subject:
              model.Subject = claim.Value;
              break;
            default:
              claims.Add(new ClaimModel
              {
                Type = claim.Type,
                Value = claim.Value,
                ValueType = claim.ValueType
              });
              break;
          }
        }

        model.Claims = claims;
      }

      return model;
    }

    private string GetAudience(Realm? realm) => (realm?.Url ?? realm?.Alias ?? _userContext.BaseUrl).ToLower();
    private string GetIssuer(Realm? realm) => (realm == null ? _userContext.BaseUrl : $"{_userContext.BaseUrl}/realms/{realm.Alias}").ToLower();
  }
}
