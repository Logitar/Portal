using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal class RecoverPasswordCommandHandler : IRequestHandler<RecoverPasswordCommand>
  {
    private readonly IRepository _repository;
    private readonly IInternalTokenService _internalTokenService;

    public RecoverPasswordCommandHandler(IRepository repository, IInternalTokenService internalTokenService)
    {
      _repository = repository;
      _internalTokenService = internalTokenService;
    }

    /// <summary>
    /// TODO(fpion): implement when Messages are completed
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException{Realm}"></exception>
    /// <exception cref="EntityNotFoundException{User}"></exception>
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

      //if (realm.PasswordRecoveryTemplate == null)
      //{
      //  throw new PasswordRecoveryTemplateRequiredException(realm);
      //}

      CreateTokenPayload createTokenPayload = new()
      {
        Lifetime = ResetPassword.Lifetime,
        Purpose = ResetPassword.Purpose,
        Realm = realm.Id.Value,
        Subject = user.Id.Value
      };
      TokenModel token = await _internalTokenService.CreateAsync(createTokenPayload, cancellationToken);

      //SendMessagePayload sendMessagePayload = new()
      //{
      //  Realm = realm.Id.Value,
      //  Template = realm.PasswordRecoveryTemplate.Id.Value,
      //  SenderId = realm.PasswordRecoverySender?.Id,
      //  IgnoreUserLocale = payload.IgnoreUserLocale,
      //  Locale = payload.Locale,
      //  Recipients = new[] { new RecipientPayload { User = user.Id.Value } },
      //  Variables = new[] { new VariablePayload { Key = "Token", Value = token.Token } }
      //};
      //SentMessagesModel sentMessages = await _messageService.SendAsync(sendMessagePayload, cancellationToken);

      //if (sentMessages.Error.Any() || sentMessages.Unsent.Any())
      //{
      //  StringBuilder message = new();

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
      //  StringBuilder message = new();

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

      //  throw new InvalidOperationException(message.ToString());
      //}

      return Unit.Value;
    }
  }
}
