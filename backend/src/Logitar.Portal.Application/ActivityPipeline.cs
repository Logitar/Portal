using Logitar.Portal.Application.Logging;
using MediatR;

namespace Logitar.Portal.Application;

internal class ActivityPipeline : IActivityPipeline
{
  private readonly ILoggingService _loggingService;
  private readonly IMediator _mediator;

  public ActivityPipeline(ILoggingService loggingService, IMediator mediator)
  {
    _loggingService = loggingService;
    _mediator = mediator;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    try
    {
      if (request is Activity activity)
      {
        // TODO(fpion): Contextualize

        _loggingService.SetActivity(activity);
      }

      return await _mediator.Send(request, cancellationToken);
    }
    catch (Exception exception)
    {
      _loggingService.Report(exception);

      throw;
    }
  }
}
