using MediatR;

namespace Logitar.Portal.Application
{
  public interface IRequestPipeline
  {
    Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
  }
}
