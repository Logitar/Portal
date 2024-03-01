namespace Logitar.Portal.MassTransit;

internal class Worker : BackgroundService
{
  private const int MillisecondsDelay = 1000;
  private const int SecondsTimeout = 2 * 60;

  private readonly TestContext _context;
  private readonly ILogger<Worker> _logger;
  private readonly IServiceProvider _serviceProvider;

  public Worker(TestContext context, ILogger<Worker> logger, IServiceProvider serviceProvider)
  {
    _context = context;
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    _logger.LogInformation("Test sequence starting.");

    IEnumerable<ITests> allTests = _serviceProvider.GetRequiredService<IEnumerable<ITests>>();
    foreach (ITests tests in allTests)
    {
      await tests.ExecuteAsync(cancellationToken);
    }

    DateTime now = DateTime.Now;
    DateTime timeout = now.AddSeconds(SecondsTimeout);
    do
    {
      if (_context.HasCompleted)
      {
        break;
      }
      _logger.LogInformation("Test sequence running... [{Percentage}%] ({Completed}/{Count})", _context.Percentage, _context.Completed, _context.Count);
      await Task.Delay(MillisecondsDelay, cancellationToken);
    } while (now < timeout || !cancellationToken.IsCancellationRequested);

    LogLevel level = _context.HasCompleted ? LogLevel.Information : LogLevel.Error;
    _logger.Log(level, "Test sequence ended. | Completed rate: {Percentage}%", _context.Percentage);
  }
}
