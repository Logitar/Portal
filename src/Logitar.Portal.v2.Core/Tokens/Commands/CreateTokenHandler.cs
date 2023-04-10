using FluentValidation;
using Logitar.Portal.v2.Contracts.Tokens;
using Logitar.Portal.v2.Core.Claims;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Tokens.Validators;
using MediatR;
using System.Security.Claims;

namespace Logitar.Portal.v2.Core.Tokens.Commands;

internal class CreateTokenHandler : IRequestHandler<CreateToken, CreatedToken>
{
  private readonly IRealmRepository _realmRepository;
  private readonly ITokenManager _tokenManager;

  public CreateTokenHandler(IRealmRepository realmRepository, ITokenManager tokenManager)
  {
    _realmRepository = realmRepository;
    _tokenManager = tokenManager;
  }

  public async Task<CreatedToken> Handle(CreateToken request, CancellationToken cancellationToken)
  {
    CreateTokenInput input = request.Input;
    new CreateTokenValidator().ValidateAndThrow(input);

    RealmAggregate? realm = input.Realm == null ? null
      : await _realmRepository.LoadAsync(input.Realm, cancellationToken)
          ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));

    // TODO(fpion): what if Portal realm URL changed?

    ClaimsIdentity identity = new();

    DateTime? expires = input.Lifetime.HasValue
      ? DateTime.UtcNow.AddSeconds(input.Lifetime.Value)
      : null;

    if (input.IsConsumable)
    {
      identity.AddClaim(CreateClaim(Rfc7519ClaimTypes.TokenId, Guid.NewGuid().ToString()));
    }

    if (input.Purpose != null)
    {
      identity.AddClaim(CreateClaim(Rfc7519ClaimTypes.Purpose, input.Purpose.ToLower()));
    }

    if (input.Subject != null)
    {
      identity.AddClaim(CreateClaim(Rfc7519ClaimTypes.Subject, input.Subject));
    }
    if (input.EmailAddress != null)
    {
      identity.AddClaim(CreateClaim(Rfc7519ClaimTypes.EmailAddress, input.EmailAddress));
    }

    if (input.Claims != null)
    {
      identity.AddClaims(input.Claims.Select(CreateClaim));
    }

    string? audience = input.Audience?.Format(realm) ?? realm?.GetAudience(); // TODO(fpion): see algorithm
    string? issuer = input.Issuer?.Format(realm) ?? realm?.GetIssuer(); // TODO(fpion): see algorithm
    string? secret = realm?.Secret ?? input.Secret ?? string.Empty; // secret override + UI validation

    return new CreatedToken
    {
      Token = _tokenManager.Create(identity, secret, input.Algorithm, expires, audience, issuer)
    };
  }

  private static Claim CreateClaim(TokenClaim claim)
    => CreateClaim(claim.Type, claim.Value, claim.ValueType);
  private static Claim CreateClaim(string type, string value, string? valueType = null)
    => new(type, value, valueType);
}
