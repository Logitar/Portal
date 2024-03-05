using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, User>
{
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;

  public AuthenticateUserCommandHandler(IUserManager userManager, IUserQuerier userQuerier)
  {
    _userManager = userManager;
    _userQuerier = userQuerier;
  }

  public async Task<User> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    TenantId? tenantId = command.TenantId;
    FoundUsers users = await _userManager.FindAsync(tenantId?.Value, payload.UniqueName, cancellationToken);
    UserAggregate user = users.SingleOrDefault() ?? throw new UserNotFoundException(tenantId, payload.UniqueName, nameof(payload.UniqueName));
    ActorId actorId = new(user.Id.Value);

    user.Authenticate(payload.Password, actorId);

    await _userManager.SaveAsync(user, command.UserSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }
}
