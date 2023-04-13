﻿using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Messages.Commands;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Tokens.Commands;
using Logitar.Portal.Core.Users.Contact;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal class RecoverPasswordHandler : IRequestHandler<RecoverPassword, SentMessages>
{
  private readonly IMediator _mediator;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserRepository _userRepository;

  public RecoverPasswordHandler(IMediator mediator,
    IRealmRepository realmRepository,
    IUserRepository userRepository)
  {
    _mediator = mediator;
    _realmRepository = realmRepository;
    _userRepository = userRepository;
  }

  public async Task<SentMessages> Handle(RecoverPassword request, CancellationToken cancellationToken)
  {
    RecoverPasswordInput input = request.Input;

    RealmAggregate realm = await _realmRepository.LoadAsync(input.Realm, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(input.Realm, nameof(input.Realm));
    if (!realm.PasswordRecoveryTemplateId.HasValue)
    {
      throw new PasswordRecoveryTemplateRequiredException(realm);
    }

    string username = input.Username.Trim();
    UserAggregate? user = await _userRepository.LoadAsync(realm, username, cancellationToken);
    if (user == null && realm.RequireUniqueEmail)
    {
      ReadOnlyEmail email = new(input.Username);
      IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(realm, email, cancellationToken);
      if (users.Count() == 1)
      {
        user = users.Single();
      }
    }

    if (user == null)
    {
      throw new InvalidCredentialsException($"The user '{username}' could not be found in realm '{realm}'.");
    }
    else if (user.IsDisabled)
    {
      throw new AccountIsDisabledException(user);
    }
    else if (realm.RequireConfirmedAccount && !user.IsConfirmed)
    {
      throw new AccountIsNotConfirmedException(user);
    }

    CreateTokenInput createToken = new()
    {
      IsConsumable = true,
      Lifetime = Constants.PasswordRecovery.Lifetime,
      Purpose = Constants.PasswordRecovery.Purpose,
      Realm = realm.Id.ToGuid().ToString(),
      Subject = user.Id.ToGuid().ToString()
    };
    CreatedToken createdToken = await _mediator.Send(new CreateToken(createToken), cancellationToken);

    SendMessageInput sendMessage = new()
    {
      Realm = realm.Id.ToGuid().ToString(),
      Template = realm.PasswordRecoveryTemplateId.Value.ToGuid().ToString(),
      SenderId = realm.PasswordRecoverySenderId?.ToGuid(),
      IgnoreUserLocale = input.IgnoreUserLocale,
      Locale = input.Locale,
      Recipients = new[]
      {
        new RecipientInput { User = user.Id.ToGuid().ToString() }
      },
      Variables = new[]
      {
        new Variable { Key = nameof(createdToken.Token), Value = createdToken.Token }
      }
    };
    SentMessages sentMessages = await _mediator.Send(new SendMessage(sendMessage), cancellationToken);

    return sentMessages;
  }
}
