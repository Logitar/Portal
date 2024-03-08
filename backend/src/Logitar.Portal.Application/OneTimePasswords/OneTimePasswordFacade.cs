using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.OneTimePasswords.Commands;
using Logitar.Portal.Application.OneTimePasswords.Queries;
using Logitar.Portal.Contracts.Passwords;

namespace Logitar.Portal.Application.OneTimePasswords;

internal class OneTimePasswordFacade : IOneTimePasswordService
{
  private readonly IActivityPipeline _activityPipeline;

  public OneTimePasswordFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<OneTimePassword> CreateAsync(CreateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateOneTimePasswordCommand(payload), cancellationToken);
  }

  public async Task<OneTimePassword?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new DeleteOneTimePasswordCommand(id), cancellationToken);
  }

  public async Task<OneTimePassword?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadOneTimePasswordQuery(id), cancellationToken);
  }

  public async Task<OneTimePassword?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ValidateOneTimePasswordCommand(id, payload), cancellationToken);
  }
}
