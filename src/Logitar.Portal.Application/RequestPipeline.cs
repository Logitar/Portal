using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Logging;
using MediatR;

namespace Logitar.Portal.Application;

internal class RequestPipeline : IRequestPipeline
{
  private readonly ILoggingService _loggingService;
  private readonly IMediator _mediator;

  public RequestPipeline(ILoggingService loggingService, IMediator mediator)
  {
    _loggingService = loggingService;
    _mediator = mediator;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    LogActivity(request);

    try
    {
      return await _mediator.Send(request, cancellationToken);
    }
    catch (Exception exception)
    {
      _loggingService.AddException(exception);

      throw;
    }
  }

  private void LogActivity<T>(IRequest<T> request)
  {
    switch (request)
    {
      case InitializeConfigurationCommand initializeConfiguration:
        initializeConfiguration = DeepClone(initializeConfiguration);
        initializeConfiguration.Payload.User.Password = Mask(initializeConfiguration.Payload.User.Password);
        _loggingService.SetActivity(initializeConfiguration);
        break;
      default:
        _loggingService.SetActivity(request);
        break;
    }
  }

  private static T DeepClone<T>(T value)
  {
    Type type = value?.GetType() ?? throw new ArgumentNullException(nameof(value));

    string json = JsonSerializer.Serialize(value, type);

    return (T?)JsonSerializer.Deserialize(json, type)
      ?? throw new InvalidOperationException($"The value could not be deserialized: '{json}'.");
  }

  private static string Mask(string s) => new(s.Select(c => '*').ToArray());
}
