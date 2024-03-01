namespace Logitar.Portal.MassTransit;

internal interface ITests
{
  Task ExecuteAsync(CancellationToken cancellationToken = default);
}
