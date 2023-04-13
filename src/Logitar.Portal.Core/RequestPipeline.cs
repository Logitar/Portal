using Logitar.Portal.Core.Logging;
using MediatR;

namespace Logitar.Portal.Core;

internal class RequestPipeline : IRequestPipeline
{
  private readonly ILoggingService _loggingService;
  private readonly IMediator _mediator;

  public RequestPipeline(ILoggingService loggingService, IMediator mediator)
  {
    _loggingService = loggingService;
    _mediator = mediator;
  }

  public async Task ExecuteAsync(IRequest request, CancellationToken cancellationToken)
  {
    Guid activityId = await _loggingService.StartActivityAsync(request, cancellationToken);

    try
    {
      await _mediator.Send(request, cancellationToken);
    }
    catch (Exception)
    {
      throw; // TODO(fpion): implement
    }

    await _loggingService.EndActivityAsync(activityId, cancellationToken);
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    Guid activityId = await _loggingService.StartActivityAsync(request, cancellationToken);

    try
    {
      return await _mediator.Send(request, cancellationToken);
    }
    catch (Exception)
    {
      throw; // TODO(fpion): implement
    }
    finally
    {
      await _loggingService.EndActivityAsync(activityId, cancellationToken);
    }
  }
}
