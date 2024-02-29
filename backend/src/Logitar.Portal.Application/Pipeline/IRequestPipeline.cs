using MediatR;

namespace Logitar.Portal.Application.Pipeline;

public interface IRequestPipeline
{
  Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
}
