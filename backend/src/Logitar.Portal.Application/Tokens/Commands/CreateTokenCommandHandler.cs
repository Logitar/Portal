using FluentValidation;
using Logitar.Portal.Application.Claims;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Realms;
using MediatR;
using System.Security.Claims;

namespace Logitar.Portal.Application.Tokens.Commands
{
  internal class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, TokenModel>
  {
    private readonly IValidator<CreateTokenPayload> _payloadValidator;
    private readonly IRealmRepository _realmRepository;
    private readonly ISecurityTokenService _securityTokenService;
    private readonly IUserContext _userContext;

    public CreateTokenCommandHandler(IValidator<CreateTokenPayload> payloadValidator,
      IRealmRepository realmRepository,
      ISecurityTokenService securityTokenService,
      IUserContext userContext)
    {
      _payloadValidator = payloadValidator;
      _realmRepository = realmRepository;
      _securityTokenService = securityTokenService;
      _userContext = userContext;
    }

    public async Task<TokenModel> Handle(CreateTokenCommand request, CancellationToken cancellationToken)
    {
      CreateTokenPayload payload = request.Payload;
      _payloadValidator.ValidateAndThrow(payload);

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _realmRepository.LoadByAliasOrIdAsync(payload.Realm, cancellationToken)
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
        ?? (await _realmRepository.LoadConfigurationAsync(cancellationToken))?.JwtSecret
        ?? throw new InvalidOperationException("The JWT secret could not be resolved.");

      string audience = ClaimHelper.GetAudience(realm, _userContext);
      string issuer = ClaimHelper.GetIssuer(realm, _userContext);
      string token = _securityTokenService.Create(identity, secret, audience, expires, issuer);

      return new TokenModel
      {
        Token = token
      };
    }
  }
}
