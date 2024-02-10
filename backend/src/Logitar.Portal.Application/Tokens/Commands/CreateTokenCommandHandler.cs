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
  private readonly IApplicationContext _applicationContext;
  private readonly ITokenManager _tokenManager;

  public CreateTokenCommandHandler(IApplicationContext applicationContext, ITokenManager tokenManager)
  {
    _applicationContext = applicationContext;
    _tokenManager = tokenManager;
  }

  public async Task<Contracts.Tokens.CreatedToken> Handle(CreateTokenCommand command, CancellationToken cancellationToken)
  {
    CreateTokenPayload payload = command.Payload;
    new CreateTokenValidator().ValidateAndThrow(payload);

    Realm? realm = _applicationContext.Realm;
    string baseUrl = _applicationContext.BaseUrl.TrimEnd('/');

    ClaimsIdentity subject = CreateSubject(payload);
    string? secret = payload.Secret?.CleanTrim() ?? realm?.Secret ?? _applicationContext.Configuration.Secret;
    CreateTokenParameters parameters = new(subject, secret)
    {
      Audience = ResolveAudience(payload.Audience, realm, baseUrl),
      Issuer = ResolveIssuer(payload.Issuer, realm, baseUrl)
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

  private static string ResolveAudience(string? audience, Realm? realm, string baseUrl)
  {
    if (!string.IsNullOrWhiteSpace(audience))
    {
      return FormatAudienceOrIssuer(audience.Trim(), realm, baseUrl);
    }
    else if (realm != null)
    {
      return realm.Url ?? realm.UniqueSlug;
    }

    return baseUrl;
  }
  private static string ResolveIssuer(string? issuer, Realm? realm, string baseUrl)
  {
    if (!string.IsNullOrWhiteSpace(issuer))
    {
      return FormatAudienceOrIssuer(issuer.Trim(), realm, baseUrl);
    }
    else if (realm != null)
    {
      return realm.Url ?? FormatAudienceOrIssuer("{BaseUrl}/realms/unique-slug:{UniqueSlug}", realm, baseUrl);
    }

    return baseUrl;
  }

  private static string FormatAudienceOrIssuer(string format, Realm? realm, string baseUrl)
  {
    if (realm != null)
    {
      format = format.Replace("{Id}", realm.Id.ToString())
        .Replace("{UniqueSlug}", realm.UniqueSlug)
        .Replace("{Url}", realm.Url);
    }

    return format.Replace("{BaseUrl}", baseUrl);
  }
}
