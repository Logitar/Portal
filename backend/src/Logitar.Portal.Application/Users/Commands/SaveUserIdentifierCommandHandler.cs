using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class SaveUserIdentifierCommandHandler : IRequestHandler<SaveUserIdentifierCommand, User?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SaveUserIdentifierCommandHandler(IApplicationContext applicationContext,
    IUserManager userManager, IUserQuerier userQuerier, IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userManager = userManager;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User?> Handle(SaveUserIdentifierCommand command, CancellationToken cancellationToken)
  {
    CustomIdentifier identifier = new(command.Key, command.Payload.Value);
    new CustomIdentifierContractValidator().ValidateAndThrow(identifier);

    UserAggregate? user = await _userRepository.LoadAsync(command.Id, cancellationToken);
    if (user == null || user.TenantId != _applicationContext.TenantId)
    {
      return null;
    }
    ActorId actorId = _applicationContext.ActorId;

    user.SetCustomIdentifier(identifier.Key, identifier.Value, actorId);

    await _userManager.SaveAsync(user, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
