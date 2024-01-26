using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPasswordManager _passwordManager;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public CreateUserCommandHandler(IApplicationContext applicationContext, IPasswordManager passwordManager,
    IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _passwordManager = passwordManager;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(CreateUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = _applicationContext.UserSettings;

    CreateUserPayload payload = command.Payload;
    new CreateUserValidator(userSettings).ValidateAndThrow(payload);

    UserId? userId = UserId.TryCreate(payload.Id);
    if (userId != null && _userRepository.LoadAsync(userId, includeDeleted: true, cancellationToken) != null)
    {
      throw new IdentifierAlreadyUsedException<UserAggregate>(payload.Id!, nameof(payload.Id));
    }

    UniqueNameUnit uniqueName = new(userSettings.UniqueName, payload.UniqueName);
    TenantId? tenantId = TenantId.TryCreate(_applicationContext.Realm?.Id);
    ActorId actorId = _applicationContext.ActorId;
    UserAggregate user = new(uniqueName, tenantId, actorId, userId);

    if (payload.Password != null)
    {
      Password password = _passwordManager.ValidateAndCreate(payload.Password);
      user.SetPassword(password, actorId);
    }

    if (payload.IsDisabled)
    {
      user.Disable(actorId);
    }

    await _userManager.SaveAsync(user, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
