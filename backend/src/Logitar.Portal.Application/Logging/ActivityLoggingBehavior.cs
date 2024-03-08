using MediatR;

namespace Logitar.Portal.Application.Logging;

internal class ActivityLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
  private readonly ILoggingService _loggingService;

  public ActivityLoggingBehavior(ILoggingService loggingService)
  {
    _loggingService = loggingService;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (request is Activity activity)
    {
      _loggingService.SetActivity(activity);
    }

    try
    {
      return await next();
    }
    catch (Exception exception)
    {
      _loggingService.Report(exception);

      throw;
    }
  }
} // TODO(fpion): remove this class
