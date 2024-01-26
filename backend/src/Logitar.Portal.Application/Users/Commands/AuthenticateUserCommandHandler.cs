using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;

  public AuthenticateUserCommandHandler(IApplicationContext applicationContext, IUserManager userManager, IUserQuerier userQuerier)
  {
    _applicationContext = applicationContext;
    _userManager = userManager;    _userQuerier = userQuerier;

  }

  public async Task<User> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    string? tenantId = _applicationContext.Realm?.Id;
    FoundUsers users = await _userManager.FindAsync(tenantId, payload.UniqueName, cancellationToken);
    UserAggregate user = users.FirstOrDefault() ?? throw new UserNotFoundException(payload.UniqueName, nameof(payload.UniqueName));

    ActorId actorId = new(user.Id.Value);
    user.Authenticate(payload.Password, actorId);

    await _userManager.SaveAsync(user, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(user, cancellationToken);
  }
}
