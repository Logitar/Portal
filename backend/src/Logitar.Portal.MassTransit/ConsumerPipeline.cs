using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Logging;
using Logitar.Portal.MassTransit.Settings;
using MassTransit;
using MediatR;

namespace Logitar.Portal.MassTransit;

internal class ConsumerPipeline : IConsumerPipeline
{
  private const string OperationType = nameof(MassTransit);

  private readonly IActivityPipeline _activityPipeline;
  private readonly ILoggingService _loggingService;
  private readonly IMassTransitSettings _settings;

  public ConsumerPipeline(IActivityPipeline activityPipeline, ILoggingService loggingService, IMassTransitSettings settings)
  {
    _activityPipeline = activityPipeline;
    _loggingService = loggingService;
    _settings = settings;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, Type consumerType, ConsumeContext context)
  {
    CancellationToken cancellationToken = context.CancellationToken;

    string? correlationId = context.CorrelationId?.ToString();
    string? method = _settings.TransportBroker?.ToString() ?? OperationType;
    string? destination = context.DestinationAddress?.ToString();
    string? source = context.SourceAddress?.ToString();
    string? additionalInformation = JsonSerializer.Serialize(context.Headers, context.Headers.GetType());
    _loggingService.Open(correlationId, method, destination, source, additionalInformation);

    Operation operation = new(OperationType, consumerType.Name);
    _loggingService.SetOperation(operation);

    int statusCode = (int)HttpStatusCode.OK;
    try
    {
      return await _activityPipeline.ExecuteAsync(request, context.GetParameters(), cancellationToken);
    }
    catch (Exception exception)
    {
      statusCode = (int)HttpStatusCode.InternalServerError;

      _loggingService.Report(exception);

      throw;
    }
    finally
    {
      await _loggingService.CloseAndSaveAsync(statusCode, cancellationToken);
    }
  }
}
