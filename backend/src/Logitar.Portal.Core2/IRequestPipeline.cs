using MediatR;

namespace Logitar.Portal.Core2
{
  public interface IRequestPipeline
  {
    Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
  }
}
