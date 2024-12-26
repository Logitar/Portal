using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Passwords.Commands;
using Logitar.Portal.Application.Passwords.Queries;
using Logitar.Portal.Contracts.Passwords;

namespace Logitar.Portal.Application.Passwords;

internal class OneTimePasswordFacade : IOneTimePasswordService
{
  private readonly IActivityPipeline _activityPipeline;

  public OneTimePasswordFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<OneTimePasswordModel> CreateAsync(CreateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new CreateOneTimePasswordCommand(payload), cancellationToken);
  }

  public async Task<OneTimePasswordModel?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new DeleteOneTimePasswordCommand(id), cancellationToken);
  }

  public async Task<OneTimePasswordModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadOneTimePasswordQuery(id), cancellationToken);
  }

  public async Task<OneTimePasswordModel?> ValidateAsync(Guid id, ValidateOneTimePasswordPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ValidateOneTimePasswordCommand(id, payload), cancellationToken);
  }
}
