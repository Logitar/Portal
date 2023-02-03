using FluentValidation;
using Logitar.Portal.Application.Claims;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Realms;
using System.Security.Claims;

namespace Logitar.Portal.Application.Tokens
{
  internal class InternalTokenService : IInternalTokenService
  {
    private readonly IValidator<CreateTokenPayload> _payloadValidator;
    private readonly IRepository _repository;
    private readonly ISecurityTokenService _securityTokenService;
    private readonly IUserContext _userContext;

    public InternalTokenService(IValidator<CreateTokenPayload> payloadValidator,
      IRepository repository,
      ISecurityTokenService securityTokenService,
      IUserContext userContext)
    {
      _payloadValidator = payloadValidator;
      _repository = repository;
      _securityTokenService = securityTokenService;
      _userContext = userContext;
    }

    public async Task<TokenModel> CreateAsync(CreateTokenPayload payload, CancellationToken cancellationToken)
    {
      _payloadValidator.ValidateAndThrow(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _repository.LoadRealmByAliasOrIdAsync(payload.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      ClaimsIdentity identity = new();
      identity.AddClaim(new(Rfc7519ClaimTypes.JwtId, Guid.NewGuid().ToString()));

      DateTime? expires = payload.Lifetime.HasValue
        ? DateTime.UtcNow.AddSeconds(payload.Lifetime.Value)
        : null;

      if (payload.Purpose != null)
      {
        identity.AddClaim(new(CustomClaimTypes.Purpose, payload.Purpose.ToLower()));
      }

      if (payload.Email != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.Email, payload.Email));
      }

      if (payload.Subject != null)
      {
        identity.AddClaim(new(Rfc7519ClaimTypes.Subject, payload.Subject));
      }

      if (payload.Claims != null)
      {
        identity.AddClaims(payload.Claims.Select(claim => new Claim(claim.Type, claim.Value, claim.ValueType)));
      }

      string secret = realm?.JwtSecret
        ?? (await _repository.LoadConfigurationAsync(cancellationToken))?.JwtSecret
        ?? throw new InvalidOperationException("The JWT secret could not be resolved.");

      string token = _securityTokenService.Create(identity, secret, GetAudience(realm), expires, GetIssuer(realm));

      return new TokenModel
      {
        Token = token
      };
    }

    public async Task<ValidatedTokenModel> ValidateAsync(ValidateTokenPayload payload, bool consume, CancellationToken cancellationToken)
    {
      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _repository.LoadRealmByAliasOrIdAsync(payload.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      string secret = realm?.JwtSecret
        ?? (await _repository.LoadConfigurationAsync(cancellationToken))?.JwtSecret
        ?? throw new InvalidOperationException("The JWT secret could not be resolved.");

      ValidateTokenResult result = await _securityTokenService
        .ValidateAsync(payload.Token, secret, GetAudience(realm), GetIssuer(realm), payload.Purpose, consume, cancellationToken);

      ValidatedTokenModel model = new();
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
        List<ClaimModel> claims = new(capacity: result.Principal.Claims.Count());

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

    public string GetAudience(Realm? realm) => (realm?.Url ?? realm?.Alias ?? _userContext.BaseUrl).ToLower();
    public string GetIssuer(Realm? realm) => (realm == null ? _userContext.BaseUrl : $"{_userContext.BaseUrl}/realms/{realm.Alias}").ToLower();
  }
}
