using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public UpdateUserCommandHandler(IApplicationContext applicationContext, IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
  {
    IUserSettings userSettings = _applicationContext.UserSettings;

    UpdateUserPayload payload = command.Payload;
    new UpdateUserValidator(userSettings).ValidateAndThrow(payload);

    UserId userId = new(command.Id);
    UserAggregate? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null)
    {
      return null;
    }
    // TODO(fpion): ensure User is in current Realm

    ActorId actorId = _applicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueNameUnit uniqueName = new(userSettings.UniqueName, payload.UniqueName);
      user.SetUniqueName(uniqueName, actorId);
    }

    if (payload.IsDisabled.HasValue)
    {
      if (payload.IsDisabled.Value)
      {
        user.Disable(actorId);
      }
      else
      {
        user.Enable(actorId);
      }
    }

    await _userManager.SaveAsync(user, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
