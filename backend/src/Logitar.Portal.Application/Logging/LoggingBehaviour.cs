using MediatR;

namespace Logitar.Portal.Application.Logging;

internal class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
  private readonly ILoggingService _loggingService;

  public LoggingBehaviour(ILoggingService loggingService)
  {
    _loggingService = loggingService;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    object? activity = request.GetActivity();
    if (activity != null)
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
}
