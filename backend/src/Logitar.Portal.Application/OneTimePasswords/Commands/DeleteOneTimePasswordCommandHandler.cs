using Logitar.Identity.Domain.Passwords;
using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Commands;

internal class DeleteOneTimePasswordCommandHandler : IRequestHandler<DeleteOneTimePasswordCommand, OneTimePassword?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;
  private readonly IOneTimePasswordRepository _oneTimePasswordRepository;

  public DeleteOneTimePasswordCommandHandler(IApplicationContext applicationContext,
    IOneTimePasswordQuerier oneTimePasswordQuerier, IOneTimePasswordRepository oneTimePasswordRepository)
  {
    _applicationContext = applicationContext;
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
    _oneTimePasswordRepository = oneTimePasswordRepository;
  }

  public async Task<OneTimePassword?> Handle(DeleteOneTimePasswordCommand command, CancellationToken cancellationToken)
  {
    OneTimePasswordAggregate? oneTimePassword = await _oneTimePasswordRepository.LoadAsync(command.Id, cancellationToken);
    if (oneTimePassword == null || oneTimePassword.TenantId != _applicationContext.TenantId)
    {
      return null;
    }
    OneTimePassword result = await _oneTimePasswordQuerier.ReadAsync(oneTimePassword, cancellationToken);

    oneTimePassword.Delete(_applicationContext.ActorId);

    await _oneTimePasswordRepository.SaveAsync(oneTimePassword, cancellationToken);

    return result;
  }
}
