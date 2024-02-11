using Logitar.Portal.Application.OneTimePasswords.Commands;
using Logitar.Portal.Application.OneTimePasswords.Queries;
using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords;

internal class OneTimePasswordFacade : IOneTimePasswordService
{
  private readonly IMediator _mediator;

  public OneTimePasswordFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<OneTimePassword> CreateAsync(CreateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new CreateOneTimePasswordCommand(payload), cancellationToken);
  }

  public async Task<OneTimePassword?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new DeleteOneTimePasswordCommand(id), cancellationToken);
  }

  public async Task<OneTimePassword?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadOneTimePasswordQuery(id), cancellationToken);
  }

  public async Task<OneTimePassword?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ValidateOneTimePasswordCommand(id, payload), cancellationToken);
  }
}
