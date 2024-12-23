﻿using FluentValidation;
using Logitar.Identity.Core.Tokens;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Tokens.Validators;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands;

public record ValidateTokenCommand(ValidateTokenPayload Payload) : Activity, IRequest<ValidatedTokenModel>;

internal class ValidateTokenCommandHandler : IRequestHandler<ValidateTokenCommand, ValidatedTokenModel>
{
  private readonly IBaseUrl _baseUrl;
  private readonly ITokenManager _tokenManager;

  public ValidateTokenCommandHandler(IBaseUrl baseUrl, ITokenManager tokenManager)
  {
    _baseUrl = baseUrl;
    _tokenManager = tokenManager;
  }

  public async Task<ValidatedTokenModel> Handle(ValidateTokenCommand command, CancellationToken cancellationToken)
  {
    ValidateTokenPayload payload = command.Payload;
    new ValidateTokenValidator().ValidateAndThrow(payload);

    RealmModel? realm = command.Realm;
    string baseUrl = _baseUrl.Value;

    string secret = payload.Secret?.CleanTrim() ?? command.Secret;
    ValidateTokenParameters parameters = new(payload.Token, secret)
    {
      ValidAudiences = [TokenHelper.ResolveAudience(payload.Audience, realm, baseUrl)],
      ValidIssuers = [TokenHelper.ResolveIssuer(payload.Issuer, realm, baseUrl)],
      Consume = payload.Consume
    };
    if (!string.IsNullOrWhiteSpace(payload.Type))
    {
      parameters.ValidTypes.Add(payload.Type.Trim());
    }

    ValidatedToken validatedToken = await _tokenManager.ValidateAsync(parameters, cancellationToken);
    return CreateResult(validatedToken.ClaimsPrincipal);
  }

  private static ValidatedTokenModel CreateResult(ClaimsPrincipal principal)
  {
    ValidatedTokenModel result = new();

    string? emailAddress = null;
    bool? isEmailVerified = null;
    foreach (Claim claim in principal.Claims)
    {

      switch (claim.Type)
      {
        case Rfc7519ClaimNames.EmailAddress:
          emailAddress = claim.Value;
          break;
        case Rfc7519ClaimNames.IsEmailVerified:
          isEmailVerified = bool.Parse(claim.Value);
          break;
        case Rfc7519ClaimNames.Subject:
          result.Subject = claim.Value;
          break;
        default:
          result.Claims.Add(new(claim.Type, claim.Value, claim.ValueType));
          break;
      }
    }
    if (emailAddress != null)
    {
      result.Email = new EmailModel(emailAddress)
      {
        IsVerified = isEmailVerified ?? false
      };
    }
    else if (isEmailVerified.HasValue)
    {
      result.Claims.Add(new(Rfc7519ClaimNames.IsEmailVerified, isEmailVerified.Value.ToString()));
    }

    return result;
  }
}
