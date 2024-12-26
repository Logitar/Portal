using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Application.Users.Validators;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

public record AuthenticateUserCommand(AuthenticateUserPayload Payload) : Activity, IRequest<UserModel>
{
  public override IActivity Anonymize()
  {
    AuthenticateUserCommand command = this.DeepClone();
    command.Payload.Password = Payload.Password.Mask();
    return command;
  }
}

internal class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, UserModel>
{
  private readonly IMediator _mediator;
  private readonly IUserManager _userManager;
  private readonly IUserQuerier _userQuerier;

  public AuthenticateUserCommandHandler(IMediator mediator, IUserManager userManager, IUserQuerier userQuerier)
  {
    _mediator = mediator;
    _userManager = userManager;
    _userQuerier = userQuerier;
  }

  public async Task<UserModel> Handle(AuthenticateUserCommand command, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = command.Payload;
    new AuthenticateUserValidator().ValidateAndThrow(payload);

    FindUserQuery query = new(command.TenantId, payload.UniqueName, command.UserSettings, nameof(payload.UniqueName));
    UserAggregate user = await _mediator.Send(query, cancellationToken);
    ActorId actorId = new(user.Id.Value);

    user.Authenticate(payload.Password, actorId);

    await _userManager.SaveAsync(user, command.UserSettings, actorId, cancellationToken);

    return await _userQuerier.ReadAsync(command.Realm, user, cancellationToken);
  }
}
