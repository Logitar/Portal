namespace Logitar.Portal.Contracts;

public interface IRequestContext
{
  string? User { get; }
  CancellationToken CancellationToken { get; }
}
