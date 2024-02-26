using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.MassTransitDemo.Worker.Caching;
using Logitar.Portal.MassTransitDemo.Worker.Settings;
using MassTransit;

namespace Logitar.Portal.MassTransitDemo.Worker;

public class Worker : BackgroundService
{
  private readonly IBus _bus;
  private readonly ICacheService _cacheService;
  private readonly IConfiguration _configuration;
  private readonly ILogger<Worker> _logger;

  public Worker(IBus bus, ICacheService cacheService, IConfiguration configuration, ILogger<Worker> logger)
  {
    _bus = bus;
    _cacheService = cacheService;
    _configuration = configuration;
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    await Task.Delay(5000, cancellationToken);

    SendMessageSettings settings = _configuration.GetSection("SendMessage").Get<SendMessageSettings>() ?? new();
    SendMessagePayload payload = new(settings.Template)
    {
      SenderId = settings.SenderId,
      IgnoreUserLocale = settings.IgnoreUserLocale,
      Locale = settings.Locale,
      IsDemo = true
    };
    payload.Recipients.AddRange(settings.Recipients);
    payload.Variables.AddRange(settings.Variables);
    SendMessageCommand command = new(payload);
    _logger.LogInformation("Send message command created.");

    Guid correlationId = NewId.NextGuid();
    await _bus.Publish(command, c => c.CorrelationId = correlationId, cancellationToken);
    _logger.LogInformation("Command published using CorrelationId={CorrelationId}.", correlationId);

    int totalDelay = 0;
    for (int i = 0; i < 10; i++)
    {
      int millisecondsDelay = (int)(Math.Pow(2, i) * 100);
      await Task.Delay(millisecondsDelay, cancellationToken);
      totalDelay += millisecondsDelay;

      SentMessages? sentMessages = _cacheService.GetSentMessages(correlationId);
      if (sentMessages == null)
      {
        _logger.LogInformation("No message has been sent yet. Waiting...");
      }
      else
      {
        _logger.LogInformation("{SentMessages} message(s) have been sent. Operation completed.", sentMessages.Ids.Count);
        return;
      }
    }

    _logger.LogError("Operation timed-out after {TotalDelay} milliseconds.", totalDelay);
  }
}
