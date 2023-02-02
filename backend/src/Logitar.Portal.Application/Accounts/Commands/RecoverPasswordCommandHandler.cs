using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Tokens;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal class RecoverPasswordCommandHandler : IRequestHandler<RecoverPasswordCommand>
  {
    private readonly IRealmRepository _realmRepository;
    private readonly IInternalTokenService _internalTokenService;
    private readonly IUserRepository _userRepository;

    public RecoverPasswordCommandHandler(IRealmRepository realmRepository,
      IInternalTokenService internalTokenService,
      IUserRepository userRepository)
    {
      _realmRepository = realmRepository;
      _internalTokenService = internalTokenService;
      _userRepository = userRepository;
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
      Realm realm = await _realmRepository.LoadByAliasOrIdAsync(request.Realm, cancellationToken)
        ?? throw new EntityNotFoundException<Realm>(request.Realm);

      RecoverPasswordPayload payload = request.Payload;

      User user = await _userRepository.LoadByUsernameAsync(payload.Username, realm, cancellationToken)
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
        Realm = realm.Id.ToString(),
        Subject = user.Id.ToString()
      };
      TokenModel token = await _internalTokenService.CreateAsync(createTokenPayload, cancellationToken);

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
