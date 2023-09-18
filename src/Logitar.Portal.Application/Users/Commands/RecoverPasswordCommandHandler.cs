using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Application.Realms;
using Logitar.Portal.Application.Tokens.Commands;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class RecoverPasswordCommandHandler : IRequestHandler<RecoverPasswordCommand, RecoverPasswordResult>
{
  private readonly IMediator _mediator;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public RecoverPasswordCommandHandler(IMediator mediator, IRealmRepository realmRepository, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _mediator = mediator;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<RecoverPasswordResult> Handle(RecoverPasswordCommand command, CancellationToken cancellationToken)
  {
    RecoverPasswordPayload payload = command.Payload;

    RealmAggregate realm = await _realmRepository.FindAsync(payload.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(payload.Realm, nameof(payload.Realm));
    if (realm.PasswordRecoveryTemplateId == null)
    {
      throw new RealmHasNoPasswordRecoveryTemplateException(realm);
    }

    string tenantId = realm.Id.Value;
    UserAggregate? user = await _userRepository.LoadAsync(tenantId, payload.UniqueName, cancellationToken);
    if (user == null && realm.RequireUniqueEmail)
    {
      Email email = new(payload.UniqueName);
      IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(tenantId, email, cancellationToken);
      if (users.Count() == 1)
      {
        user = users.Single();
      }
    }
    if (user == null)
    {
      throw new UserNotFoundException(realm, payload.UniqueName);
    }

    CreateTokenPayload createToken = new()
    {
      IsConsumable = true,
      Lifetime = PasswordRecovery.Lifetime,
      Type = PasswordRecovery.TokenType,
      Subject = user.Id.ToGuid().ToString()
    };
    CreatedToken createdToken = await _mediator.Send(new CreateTokenCommand(createToken, realm), cancellationToken);

    SendMessagePayload sendMessage = new()
    {
      Realm = realm.Id.ToGuid().ToString(),
      SenderId = realm.PasswordRecoverySenderId?.ToGuid(),
      Template = realm.PasswordRecoveryTemplateId.Value.ToGuid().ToString(),
      Recipients = new RecipientPayload[]
      {
        new()
        {
          User = user.Id.ToGuid().ToString()
        }
      },
      IgnoreUserLocale = payload.IgnoreUserLocale,
      Locale = payload.Locale,
      Variables = new Variable[]
      {
        new(PasswordRecovery.TokenKey, createdToken.Token)
      }
    };
    SentMessages sentMessages = await _mediator.Send(new SendMessageCommand(sendMessage, Realm: realm), cancellationToken);

    return new RecoverPasswordResult
    {
      MessageId = sentMessages.Ids.Single(),
      User = await _userQuerier.ReadAsync(user, cancellationToken)
    };
  }
}
