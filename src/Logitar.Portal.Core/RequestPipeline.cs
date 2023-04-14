using Logitar.Portal.Contracts.Errors;
using Logitar.Portal.Core.Logging;
using MediatR;

namespace Logitar.Portal.Core;

internal class RequestPipeline : IRequestPipeline
{
  private readonly IApplicationContext _applicationContext;
  private readonly ILoggingService _loggingService;
  private readonly IMediator _mediator;

  public RequestPipeline(IApplicationContext applicationContext,
    ILoggingService loggingService,
    IMediator mediator)
  {
    _applicationContext = applicationContext;
    _loggingService = loggingService;
    _mediator = mediator;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    Activity activity = request.GetActivity();
    Guid activityId = await _loggingService.StartActivityAsync(activity, cancellationToken);
    _applicationContext.ActivityId = activityId;

    try
    {
      return await _mediator.Send(request, cancellationToken);
    }
    catch (Exception exception)
    {
      await _loggingService.AddErrorAsync(Error.From(exception), activityId, cancellationToken);

      throw;
    }
    finally
    {
      await _loggingService.EndActivityAsync(activityId, cancellationToken);
    }
  }
}
