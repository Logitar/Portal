using Logitar.Portal.Contracts.Messages;
using MassTransit;

namespace Logitar.Portal.MassTransit.Messages;

internal class SendMessageCommandTests : ITests
{
  private readonly IBus _bus;
  private readonly IConfiguration _configuration;
  private readonly ILogger<SendMessageCommandTests> _logger;

  public SendMessageCommandTests(IBus bus, IConfiguration configuration, ILogger<SendMessageCommandTests> logger)
  {
    _bus = bus;
    _configuration = configuration;
    _logger = logger;
  }

  public async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    SendMessagePayload payload = _configuration.GetSection("SendMessagePayload").Get<SendMessagePayload>() ?? new();
    payload.IsDemo = true;

    SendMessageCommand command = new(payload);
    Guid correlationId = NewId.NextGuid();
    await _bus.Publish(command, context => context.Populate(correlationId, _configuration), cancellationToken);
    _logger.LogInformation("{CommandType} sent from correlation ID '{CorrelationId}'.", command.GetType().Name, correlationId);
  }
}
