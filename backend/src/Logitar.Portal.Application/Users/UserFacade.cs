using Logitar.Portal.Application.Users.Commands;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users;

internal class UserFacade : IUserService
{
  private readonly IMediator _mediator;

  public UserFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<User> AuthenticateAsync(AuthenticateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new AuthenticateUserCommand(payload), cancellationToken);
  }

  public async Task<User> CreateAsync(CreateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateUserCommand(payload), cancellationToken);
  }

  public async Task<User?> DeleteAsync(string id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new DeleteUserCommand(id), cancellationToken);
  }

  public async Task<User?> ReplaceAsync(string id, ReplaceUserPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReplaceUserCommand(id, payload, version), cancellationToken);
  }

  public async Task<User?> ResetPasswordAsync(string id, ResetUserPasswordPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ResetUserPasswordCommand(id, payload), cancellationToken);
  }

  public async Task<User?> SignOutAsync(string id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new SignOutUserCommand(id), cancellationToken);
  }

  public async Task<User?> UpdateAsync(string id, UpdateUserPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new UpdateUserCommand(id, payload), cancellationToken);
  }
}
