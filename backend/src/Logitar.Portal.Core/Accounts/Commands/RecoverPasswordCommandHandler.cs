using MediatR;

namespace Logitar.Portal.Core.Accounts.Commands
{
  internal class RecoverPasswordCommandHandler : IRequestHandler<RecoverPasswordCommand>
  {
    public async Task<Unit> Handle(RecoverPasswordCommand request, CancellationToken cancellationToken)
    {
      throw new NotImplementedException(); // TODO(fpion): implement

      //RealmModel realm = await _realmQuerier.GetAsync(request.Realm, cancellationToken)
      //  ?? throw EntityNotFoundException.Typed<Realm>(request.Realm, nameof(request.Realm));

      //User user = await _userQuerier.GetAsync(request.Payload.Username, realm, cancellationToken)
      //  ?? throw EntityNotFoundException.Typed<User>(request.Payload.Username, nameof(request.Payload.Username));
      //EnsureIsTrusted(user, realm);
      //EnsureHasPassword(user);

      //if (realm.PasswordRecoveryTemplate == null)
      //{
      //  throw new PasswordRecoveryTemplateRequiredException(realm);
      //}

      //CreateTokenPayload createTokenPayload = new()
      //{
      //  Lifetime = PasswordResetLifetime,
      //  Purpose = PasswordResetPurpose,
      //  Realm = realm.Id.ToString(),
      //  Subject = user.Id.ToString()
      //};
      //TokenModel token = await _tokenService.CreateAsync(createTokenPayload, cancellationToken);

      //SendMessagePayload sendMessagePayload = new()
      //{
      //  Realm = realm.Id.ToString(),
      //  Template = realm.PasswordRecoveryTemplate.Id.ToString(),
      //  SenderId = realm.PasswordRecoverySender?.Id,
      //  IgnoreUserLocale = payload.IgnoreUserLocale,
      //  Locale = payload.Locale,
      //  Recipients = new[] { new RecipientPayload { User = user.Id.ToString() } },
      //  Variables = new[] { new VariablePayload { Key = "Token", Value = token.Token } }
      //};
      //SentMessagesModel sentMessages = await _messageService.SendAsync(sendMessagePayload, cancellationToken);

      //if (sentMessages.Error.Any() || sentMessages.Unsent.Any())
      //{
      //  var message = new StringBuilder();

      //  message.AppendLine("The password recovery message sending failed.");

      //  if (sentMessages.Error.Any())
      //  {
      //    message.AppendLine($"Error IDs: {string.Join(", ", sentMessages.Error)}");
      //  }
      //  if (sentMessages.Unsent.Any())
      //  {
      //    message.AppendLine($"Unsent IDs: {string.Join(", ", sentMessages.Unsent)}");
      //  }

      //  throw new InvalidOperationException(message.ToString());
      //}
      //else if (sentMessages.Success.Count() != 1)
      //{
      //  var message = new StringBuilder();

      //  if (!sentMessages.Success.Any())
      //  {
      //    message.AppendLine("No password recovery message was sent.");
      //    message.AppendLine($"User ID: {user.Id}");
      //  }
      //  else if (sentMessages.Success.Count() > 1)
      //  {
      //    message.AppendLine("No password recovery message was sent.");
      //    message.AppendLine($"User ID: {user.Id}");
      //    message.AppendLine($"Message IDs: {string.Join(", ", sentMessages.Success)}");
      //  }
      //}
    }
  }
}
