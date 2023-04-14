using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Core.Caching;
using Logitar.Portal.Core.Claims;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Tokens.Validators;
using MediatR;
using System.Security.Claims;

namespace Logitar.Portal.Core.Tokens.Commands;

internal class ValidateTokenHandler : IRequestHandler<ValidateToken, ValidatedToken>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ICacheService _cacheService;
  private readonly IRealmRepository _realmRepository;
  private readonly ITokenManager _tokenManager;

  public ValidateTokenHandler(IApplicationContext applicationContext,
    ICacheService cacheService,
    IRealmRepository realmRepository,
    ITokenManager tokenManager)
  {
    _applicationContext = applicationContext;
    _cacheService = cacheService;
    _realmRepository = realmRepository;
    _tokenManager = tokenManager;
  }

  public async Task<ValidatedToken> Handle(ValidateToken request, CancellationToken cancellationToken)
  {
    ValidateTokenInput input = request.Input;
    new ValidateTokenValidator().ValidateAndThrow(input);

    RealmAggregate realm;
    if (input.Realm == null)
    {
      realm = _cacheService.PortalRealm ?? throw new InvalidOperationException("The Portal realm was not cached.");

      AggregateId actorId = new(Guid.Empty);
      realm.SetUrl(actorId, _applicationContext.BaseUrl);
    }
    else
    {
      realm = await _realmRepository.LoadAsync(input.Realm, cancellationToken)
        ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));
    }

    string audience = input.Audience?.Format(realm) ?? realm.GetAudience();
    string issuer = input.Issuer?.Format(realm) ?? realm.GetIssuer(_applicationContext.BaseUrl);
    string secret = input.Secret ?? realm.Secret;

    try
    {
      ClaimsPrincipal principal = await _tokenManager.ValidateAsync(input.Token, secret, audience,
        issuer, input.Purpose, request.Consume, cancellationToken);

      ValidatedToken token = new();

      List<TokenClaim> claims = new(capacity: principal.Claims.Count());
      foreach (Claim claim in principal.Claims)
      {
        switch (claim.Type)
        {
          case Rfc7519ClaimTypes.EmailAddress:
            token.EmailAddress = claim.Value;
            break;
          case Rfc7519ClaimTypes.Subject:
            token.Subject = claim.Value;
            break;
          default:
            claims.Add(new TokenClaim
            {
              Type = claim.Type,
              Value = claim.Value,
              ValueType = claim.ValueType
            });
            break;
        }
      }

      token.Claims = claims;

      return token;
    }
    catch (Exception exception)
    {
      return new ValidatedToken
      {
        Errors = new[] { Error.From(exception) }
      };
    }
  }
}
