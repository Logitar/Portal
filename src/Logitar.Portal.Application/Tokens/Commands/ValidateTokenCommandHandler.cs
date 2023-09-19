using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Settings;
using Logitar.Security.Claims;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands;

internal class ValidateTokenCommandHandler : IRequestHandler<ValidateTokenCommand, ValidatedToken>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly ITokenBlacklist _tokenBlacklist;
  private readonly ITokenManager _tokenManager;

  public ValidateTokenCommandHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository, ITokenBlacklist tokenBlacklist, ITokenManager tokenManager)
  {
    _applicationContext = applicationContext;
    _realmRepository = realmRepository;
    _tokenBlacklist = tokenBlacklist;
    _tokenManager = tokenManager;
  }

  public async Task<ValidatedToken> Handle(ValidateTokenCommand command, CancellationToken cancellationToken)
  {
    ValidateTokenPayload payload = command.Payload;
    JwtSecret? secret = string.IsNullOrWhiteSpace(payload.Secret) ? null : new(payload.Secret);

    ConfigurationAggregate configuration = _applicationContext.Configuration;
    RealmAggregate? realm = command.Realm;
    if (realm == null && !string.IsNullOrWhiteSpace(payload.Realm))
    {
      realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
       ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    }

    secret ??= realm?.Secret ?? configuration.Secret;
    string? audience = TokenHelper.GetAudience(payload.Audience, realm, _applicationContext.BaseUrl);
    string? issuer = TokenHelper.GetIssuer(payload.Issuer, realm, _applicationContext.BaseUrl);
    string? type = payload.Type?.CleanTrim();

    ClaimsPrincipal principal = _tokenManager.Validate(payload.Token, secret.Value, audience, issuer, type);

    IEnumerable<Guid> ids = principal.FindAll(Rfc7519ClaimNames.TokenId).Select(x => Guid.Parse(x.Value));
    if (ids.Any())
    {
      IEnumerable<Guid> blacklisted = await _tokenBlacklist.FindBlacklistedAsync(ids, cancellationToken);
      if (blacklisted.Any())
      {
        throw new SecurityTokenBlacklistedException(blacklisted);
      }
    }

    if (payload.Consume)
    {
      DateTime? expiresOn = null;
      System.Security.Claims.Claim? claim = principal.FindFirst(Rfc7519ClaimNames.ExpirationTime);
      if (claim != null)
      {
        expiresOn = ClaimHelper.ExtractDateTime(claim).AddHours(1);
      }

      await _tokenBlacklist.BlacklistAsync(ids, expiresOn, cancellationToken);
    }

    ValidatedToken result = new();

    List<Contracts.Tokens.Claim> claims = new(capacity: principal.Claims.Count());
    foreach (System.Security.Claims.Claim claim in principal.Claims)
    {
      switch (claim.Type)
      {
        case Rfc7519ClaimNames.EmailAddress:
          result.EmailAddress = claim.Value;
          break;
        case Rfc7519ClaimNames.Subject:
          result.Subject = claim.Value;
          break;
        default:
          claims.Add(new()
          {
            Name = claim.Type,
            Value = claim.Value,
            Type = claim.ValueType
          });
          break;
      }
    }

    result.Claims = claims.AsReadOnly();

    return result;
  }
}
