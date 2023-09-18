using FluentValidation;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Settings;
using Logitar.Portal.Domain.Users;
using Logitar.Security.Claims;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands;

internal class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, CreatedToken>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ITokenManager _tokenManager;

  public CreateTokenCommandHandler(IApplicationContext applicationContext, IRealmRepository realmRepository, ITokenManager tokenManager)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _tokenManager = tokenManager;
  }

  public async Task<CreatedToken> Handle(CreateTokenCommand command, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = command.Payload;
    JwtSecret? secret = string.IsNullOrWhiteSpace(payload.Secret) ? null : new(payload.Secret);

    ConfigurationAggregate configuration = _applicationContext.Configuration;
    RealmAggregate? realm = command.Realm;
    if (realm == null && !string.IsNullOrWhiteSpace(payload.Realm))
    {
      realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
       ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    }

    ClaimsIdentity identity = new();

    if (payload.IsConsumable)
    {
      identity.AddClaim(new(Rfc7519ClaimNames.TokenId, Guid.NewGuid().ToString()));
    }

    if (!string.IsNullOrWhiteSpace(payload.Subject))
    {
      identity.AddClaim(new(Rfc7519ClaimNames.Subject, payload.Subject.Trim()));
    }
    if (!string.IsNullOrWhiteSpace(payload.EmailAddress))
    {
      EmailAddress email = new(payload.EmailAddress);
      identity.AddClaim(new(Rfc7519ClaimNames.EmailAddress, email.Address));
    }

    identity.AddClaims(payload.Claims.Where(claim => !string.IsNullOrWhiteSpace(claim.Name) && !string.IsNullOrWhiteSpace(claim.Value))
      .Select(claim => new System.Security.Claims.Claim(FormatClaimName(claim.Name), claim.Value.Trim(), claim.Type?.CleanTrim())));

    secret ??= realm?.Secret ?? configuration.Secret;
    DateTime? expires = payload.Lifetime > 0 ? DateTime.UtcNow.AddSeconds(payload.Lifetime) : null;
    string? algorithm = payload.Algorithm?.CleanTrim();
    string? audience = TokenHelper.GetAudience(payload.Audience, realm, _applicationContext.BaseUrl);
    string? issuer = TokenHelper.GetIssuer(payload.Issuer, realm, _applicationContext.BaseUrl);
    string? type = payload.Type?.CleanTrim();

    string token = _tokenManager.Create(identity, secret.Value, expires, algorithm, audience, issuer, type);

    return new CreatedToken(token);
  }

  private static string FormatClaimName(string name) => new string(name.Where(c => char.IsLetter(c) || c == '_').ToArray()).ToLower();
}
