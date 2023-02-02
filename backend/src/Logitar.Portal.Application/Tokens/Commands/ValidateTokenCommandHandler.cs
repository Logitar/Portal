using Logitar.Portal.Application.Claims;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Realms;
using MediatR;
using System.Security.Claims;

namespace Logitar.Portal.Application.Tokens.Commands
{
  internal class ValidateTokenCommandHandler : IRequestHandler<ValidateTokenCommand, ValidatedTokenModel>
  {
    private readonly IRealmRepository _realmRepository;
    private readonly ISecurityTokenService _securityTokenService;
    private readonly IUserContext _userContext;

    public ValidateTokenCommandHandler(IRealmRepository realmRepository,
      ISecurityTokenService securityTokenService,
      IUserContext userContext)
    {
      _realmRepository = realmRepository;
      _securityTokenService = securityTokenService;
      _userContext = userContext;
    }

    public async Task<ValidatedTokenModel> Handle(ValidateTokenCommand request, CancellationToken cancellationToken)
    {
      ValidateTokenPayload payload = request.Payload;

      Realm? realm = null;
      if (payload.Realm != null)
      {
        realm = await _realmRepository.LoadByAliasOrIdAsync(payload.Realm, cancellationToken)
          ?? throw new EntityNotFoundException<Realm>(payload.Realm, nameof(payload.Realm));
      }

      string secret = realm?.JwtSecret
        ?? (await _realmRepository.LoadConfigurationAsync(cancellationToken))?.JwtSecret
        ?? throw new InvalidOperationException("The JWT secret could not be resolved.");

      string audience = ClaimHelper.GetAudience(realm, _userContext);
      string issuer = ClaimHelper.GetIssuer(realm, _userContext);
      ValidateTokenResult result = await _securityTokenService
        .ValidateAsync(payload.Token, secret, audience, issuer, payload.Purpose, request.Consume, cancellationToken);

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
  }
}
