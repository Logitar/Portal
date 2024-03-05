using FluentValidation;
using Logitar.Identity.Domain.Tokens;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Tokens.Validators;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Security.Claims;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands;

internal class CreateTokenCommandHandler : IRequestHandler<CreateTokenCommand, Contracts.Tokens.CreatedToken>
{
  private readonly IBaseUrl _baseUrl;
  private readonly ITokenManager _tokenManager;

  public CreateTokenCommandHandler(IBaseUrl baseUrl, ITokenManager tokenManager)
  {
    _baseUrl = baseUrl;
    _tokenManager = tokenManager;
  }

  public async Task<Contracts.Tokens.CreatedToken> Handle(CreateTokenCommand command, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = command.Payload;
    new CreateTokenValidator().ValidateAndThrow(payload);

    Realm? realm = command.Realm;
    string baseUrl = _baseUrl.Value;

    ClaimsIdentity subject = CreateSubject(payload);
    string? secret = payload.Secret?.CleanTrim() ?? command.Secret;
    CreateTokenParameters parameters = new(subject, secret)
    {
      Audience = TokenHelper.ResolveAudience(payload.Audience, realm, baseUrl),
      Issuer = TokenHelper.ResolveIssuer(payload.Issuer, realm, baseUrl)
    };
    if (!string.IsNullOrWhiteSpace(payload.Algorithm))
    {
      parameters.SigningAlgorithm = payload.Algorithm.Trim();
    }
    if (!string.IsNullOrWhiteSpace(payload.Type))
    {
      parameters.Type = payload.Type.Trim();
    }
    if (payload.LifetimeSeconds.HasValue)
    {
      parameters.Expires = DateTime.UtcNow.AddSeconds(payload.LifetimeSeconds.Value);
    }

    Identity.Domain.Tokens.CreatedToken createdToken = await _tokenManager.CreateAsync(parameters, cancellationToken);
    return new Contracts.Tokens.CreatedToken(createdToken.TokenString);
  }

  private static ClaimsIdentity CreateSubject(CreateTokenPayload payload)
  {
    ClaimsIdentity subject = new();

    if (payload.IsConsumable)
    {
      subject.AddClaim(new(Rfc7519ClaimNames.TokenId, Guid.NewGuid().ToString()));
    }

    if (!string.IsNullOrWhiteSpace(payload.Subject))
    {
      subject.AddClaim(new(Rfc7519ClaimNames.Subject, payload.Subject.Trim()));
    }

    if (payload.Email != null)
    {
      EmailUnit email = payload.Email.ToEmailUnit();
      subject.AddClaim(new(Rfc7519ClaimNames.EmailAddress, email.Address));
      subject.AddClaim(new(Rfc7519ClaimNames.IsEmailVerified, email.IsVerified.ToString()));
    }

    foreach (TokenClaim claim in payload.Claims)
    {
      subject.AddClaim(new(claim.Name, claim.Value, claim.Type));
    }

    return subject;
  }
}
