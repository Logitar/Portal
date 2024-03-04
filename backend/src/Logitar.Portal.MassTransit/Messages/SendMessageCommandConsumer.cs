using Logitar.Portal.Application.Logging;
using Logitar.Portal.Application.Messages.Commands;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Logitar.Portal.MassTransit.Messages;

internal class SendMessageCommandConsumer : IConsumer<SendMessageCommand>
{
  private readonly IBus _bus;
  private readonly ILogger<SendMessageCommandConsumer> _logger;
  private readonly ILoggingService _loggingService;
  private readonly IMediator _mediator;
  private readonly IPopulateRequest _populateRequest;

  public SendMessageCommandConsumer(IBus bus, ILogger<SendMessageCommandConsumer> logger,
    ILoggingService loggingService, IMediator mediator, IPopulateRequest populateRequest)
  {
    _bus = bus;
    _logger = logger;
    _loggingService = loggingService;
    _mediator = mediator;
    _populateRequest = populateRequest;
  }

  public async Task Consume(ConsumeContext<SendMessageCommand> context)
  {
    Guid? correlationId = context.CorrelationId;
    SendMessageCommand command = context.Message;
    CancellationToken cancellationToken = context.CancellationToken;
    _logger.LogInformation("Consuming {CommandType} from CorrelationId '{CorrelationId}'.", command.GetType().Name, correlationId?.ToString() ?? "<null>");

    string? destination = context.DestinationAddress?.ToString();
    string? source = context.SourceAddress?.ToString();
    string? additionalInformation = JsonSerializer.Serialize(context.Headers, context.Headers.GetType());
    _loggingService.Open(correlationId?.ToString(), method: null, destination, source, additionalInformation);

    Operation operation = new(nameof(MassTransit), GetType().Name); // TODO(fpion): gets overriden by CompileTemplateCommand
    _loggingService.SetOperation(operation);

    try
    {
      SendMessageInternalCommand request = new(command.Payload);
      await _populateRequest.ExecuteAsync(context, request);

      if (request.Realm != null)
      {
        _loggingService.SetRealm(request.Realm);
      }
      if (request.Actor.Type == ActorType.User)
      {
        User user = new()
        {
          Id = request.Actor.Id
        };
        _loggingService.SetUser(user);
      }

      SentMessages sentMessages = await _mediator.Send(request, cancellationToken);
      MessagesSentEvent @event = new(sentMessages);

      await _bus.Publish(@event, c => c.CorrelationId = correlationId, cancellationToken);
      _logger.LogInformation("Published {EventType} to CorrelationId '{CorrelationId}'.", @event.GetType().Name, correlationId?.ToString() ?? "<null>");
    }
    catch (Exception exception)
    {
      _loggingService.Report(exception);

      throw;
    }
    finally
    {
      await _loggingService.CloseAndSaveAsync(statusCode: 200, cancellationToken);
    }
  }
}
