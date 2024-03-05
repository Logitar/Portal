using Logitar.Identity.Domain.Tokens;
using Logitar.Portal.Worker.Settings;

namespace Logitar.Portal.Worker.Tasks;

internal class PurgeTokenBlacklist : BackgroundService
{
  private readonly ILogger<PurgeTokenBlacklist> _logger;
  private readonly TimeSpan _period;
  private readonly IServiceProvider _serviceProvider;

  public PurgeTokenBlacklist(CronSettings cron, ILogger<PurgeTokenBlacklist> logger, IServiceProvider serviceProvider)
  {
    _logger = logger;
    _period = cron.PurgeTokenBlacklist;
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("{TaskName} starting.", GetType().Name);

    await DoWorkAsync(cancellationToken);

    using PeriodicTimer timer = new(_period);
    try
    {
      while (await timer.WaitForNextTickAsync(cancellationToken))
      {
        await DoWorkAsync(cancellationToken);
      }
    }
    catch (OperationCanceledException)
    {
      _logger.LogInformation("{TaskName} stopping.", GetType().Name);
    }
  }

  private async Task DoWorkAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("{TaskName} executing.", GetType().Name);

    using IServiceScope scope = _serviceProvider.CreateScope();
    ITokenBlacklist tokenBlacklist = scope.ServiceProvider.GetRequiredService<ITokenBlacklist>();
    await tokenBlacklist.PurgeAsync(cancellationToken);
  }
}
