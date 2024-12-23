using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record SaveUserIdentifierCommand(Guid Id, string Key, SaveUserIdentifierPayload Payload) : Activity, IRequest<UserModel?>;

internal class SaveUserIdentifierCommandHandler : IRequestHandler<SaveUserIdentifierCommand, UserModel?>
{
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SaveUserIdentifierCommandHandler(IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<UserModel?> Handle(SaveUserIdentifierCommand command, CancellationToken cancellationToken)
  {
    CustomIdentifierModel identifier = new(command.Key, command.Payload.Value);
    new CustomIdentifierContractValidator().ValidateAndThrow(identifier);

    UserId userId = new(command.TenantId, new EntityId(command.Id));
    User? user = await _userRepository.LoadAsync(userId, cancellationToken);
    if (user == null || user.TenantId != command.TenantId)
    {
      return null;
    }
    ActorId actorId = command.ActorId;

    Identifier key = new(identifier.Key); // TODO(fpion): 400 BadRequest
    CustomIdentifier value = new(identifier.Value);
    user.SetCustomIdentifier(key, value, actorId);

    await _userManager.SaveAsync(user, command.UserSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }
}
