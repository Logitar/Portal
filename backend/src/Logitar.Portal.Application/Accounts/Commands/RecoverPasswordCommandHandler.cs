using Logitar.Portal.Application.Messages;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;
using System.Text;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal class RecoverPasswordCommandHandler : IRequestHandler<RecoverPasswordCommand>
  {
    private readonly IInternalMessageService _internalMessageService;
    private readonly IInternalTokenService _internalTokenService;
    private readonly IRepository _repository;

    public RecoverPasswordCommandHandler(IInternalMessageService internalMessageService,
      IInternalTokenService internalTokenService,
      IRepository repository)
    {
      _internalMessageService = internalMessageService;
      _internalTokenService = internalTokenService;
      _repository = repository;
    }

    public async Task<Unit> Handle(RecoverPasswordCommand request, CancellationToken cancellationToken)
    {
      Realm realm = await _repository.LoadRealmByAliasOrIdAsync(request.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<Realm>(request.Realm);

      RecoverPasswordPayload payload = request.Payload;

      User user = await _repository.LoadUserByUsernameAsync(payload.Username, realm, cancellationToken)
        ?? throw new EntityNotFoundException<User>(payload.Username, nameof(payload.Username));
      user.EnsureIsTrusted(realm);
      if (!user.HasPassword)
      {
        throw new UserHasNoPasswordException(user);
      }

      if (!realm.PasswordRecoveryTemplateId.HasValue)
      {
        throw new PasswordRecoveryTemplateRequiredException(realm);
      }

      CreateTokenPayload createTokenPayload = new()
      {
        Lifetime = ResetPassword.Lifetime,
        Purpose = ResetPassword.Purpose,
        Realm = realm.Id.Value,
        Subject = user.Id.Value
      };
      TokenModel token = await _internalTokenService.CreateAsync(createTokenPayload, cancellationToken);

      SendMessagePayload sendMessagePayload = new()
      {
        Realm = realm.Id.Value,
        Template = realm.PasswordRecoveryTemplateId.Value.Value,
        SenderId = realm.PasswordRecoverySenderId?.Value,
        IgnoreUserLocale = payload.IgnoreUserLocale,
        Locale = payload.Locale,
        Recipients = new RecipientPayload[] { new() { User = user.Id.Value } },
        Variables = new VariablePayload[] { new() { Key = "Token", Value = token.Token } }
      };
      SentMessagesModel sentMessages = await _internalMessageService.SendAsync(sendMessagePayload, cancellationToken);

      if (sentMessages.Error.Any() || sentMessages.Unsent.Any())
      {
        StringBuilder message = new();

        message.AppendLine("The password recovery message sending failed.");

        if (sentMessages.Error.Any())
        {
          message.AppendLine($"Error IDs: {string.Join(", ", sentMessages.Error)}");
        }
        if (sentMessages.Unsent.Any())
        {
          message.AppendLine($"Unsent IDs: {string.Join(", ", sentMessages.Unsent)}");
        }

        throw new InvalidOperationException(message.ToString());
      }
      else if (sentMessages.Success.Count() != 1)
      {
        StringBuilder message = new();

        if (!sentMessages.Success.Any())
        {
          message.AppendLine("No password recovery message was sent.");
          message.AppendLine($"User ID: {user.Id}");
        }
        else if (sentMessages.Success.Count() > 1)
        {
          message.AppendLine("No password recovery message was sent.");
          message.AppendLine($"User ID: {user.Id}");
          message.AppendLine($"Message IDs: {string.Join(", ", sentMessages.Success)}");
        }

        throw new InvalidOperationException(message.ToString());
      }

      return Unit.Value;
    }
  }
}
