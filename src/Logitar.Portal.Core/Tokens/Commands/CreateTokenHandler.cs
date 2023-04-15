using FluentValidation;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Core.Claims;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Tokens.Validators;
using MediatR;
using System.Security.Claims;

namespace Logitar.Portal.Core.Tokens.Commands;

internal class CreateTokenHandler : IRequestHandler<CreateToken, CreatedToken>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ITokenManager _tokenManager;

  public CreateTokenHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository,
    ITokenManager tokenManager)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _tokenManager = tokenManager;
  }

  public async Task<CreatedToken> Handle(CreateToken request, CancellationToken cancellationToken)
  {
    CreateTokenInput input = request.Input;
    new CreateTokenValidator().ValidateAndThrow(input);

    RealmAggregate? realm = await LoadRealmAsync(input, cancellationToken);

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

    string audience = input.Audience?.Format(realm) ?? realm?.GetAudience()
      ?? _applicationContext.BaseUrl?.ToString() ?? Constants.DefaultIdentifier;
    string issuer = input.Issuer?.Format(realm) ?? realm?.GetIssuer(_applicationContext.BaseUrl)
      ?? _applicationContext.BaseUrl?.ToString() ?? Constants.DefaultIdentifier;
    string secret = input.Secret ?? realm?.Secret ?? _applicationContext.Configuration.Secret;

    return new CreatedToken
    {
      Token = _tokenManager.Create(identity, secret, input.Algorithm, expires, audience, issuer)
    };
  }

  private async Task<RealmAggregate?> LoadRealmAsync(CreateTokenInput input, CancellationToken cancellationToken)
  {
    if (input.Realm == null)
    {
      return null;
    }

    return await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));
  }

  private static Claim CreateClaim(TokenClaim claim)
    => CreateClaim(claim.Type, claim.Value, claim.ValueType);
  private static Claim CreateClaim(string type, string value, string? valueType = null)
    => new(type, value, valueType);
}
