﻿using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Errors;
using Logitar.Portal.v2.Contracts.Tokens;
using Logitar.Portal.v2.Core.Claims;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Tokens.Validators;
using MediatR;
using System.Security.Claims;

namespace Logitar.Portal.v2.Core.Tokens.Commands;

internal class ValidateTokenHandler : IRequestHandler<ValidateToken, ValidatedToken>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ITokenManager _tokenManager;

  public ValidateTokenHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository,
    ITokenManager tokenManager)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _tokenManager = tokenManager;
  }

  public async Task<ValidatedToken> Handle(ValidateToken request, CancellationToken cancellationToken)
  {
    ValidateTokenInput input = request.Input;
    new ValidateTokenValidator().ValidateAndThrow(input);

    RealmAggregate? realm = input.Realm == null ? null
     : await _realmRepository.LoadAsync(input.Realm, cancellationToken)
       ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));

    if (realm?.UniqueName == Constants.PortalRealm.UniqueName)
    {
      AggregateId actorId = new(Guid.Empty);
      realm.SetUrl(actorId, _applicationContext.BaseUrl);
    }

    string? audience = input.Audience?.Format(realm) ?? realm?.GetAudience();
    string? issuer = input.Issuer?.Format(realm) ?? realm?.GetIssuer(_applicationContext.BaseUrl);
    string? secret = input.Secret ?? realm?.Secret ?? string.Empty;

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
      Error error = new()
      {
        Severity = ErrorSeverity.Failure,
        Code = exception.GetType().Name.Remove(nameof(Exception)),
        Description = exception.Message
      };

      return new ValidatedToken
      {
        Errors = new[] { error }
      };
    }
  }
}
