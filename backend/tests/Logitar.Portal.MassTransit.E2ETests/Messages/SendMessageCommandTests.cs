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
    string? realm = _configuration.GetValue<string>("Realm");
    string? user = _configuration.GetValue<string>("User");

    SendMessagePayload payload = _configuration.GetSection("SendMessagePayload").Get<SendMessagePayload>() ?? new();
    payload.IsDemo = true;

    SendMessageCommand command = new(payload);
    Guid correlationId = NewId.NextGuid();
    await _bus.Publish(command, context =>
    {
      context.CorrelationId = correlationId;
      if (!string.IsNullOrWhiteSpace(realm))
      {
        context.Headers.Set(Contracts.Constants.Headers.Realm, realm.Trim());
      }
      if (!string.IsNullOrWhiteSpace(user))
      {
        context.Headers.Set(Contracts.Constants.Headers.User, user.Trim());
      }
    }, cancellationToken);
    _logger.LogInformation("{CommandType} sent from correlation ID '{CorrelationId}'.", command.GetType().Name, correlationId);
  }
}
