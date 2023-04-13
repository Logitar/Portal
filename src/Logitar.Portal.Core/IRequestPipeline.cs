using MediatR;

namespace Logitar.Portal.Core;

internal interface IRequestPipeline
{
  Task ExecuteAsync(IRequest request, CancellationToken cancellationToken = default);
  Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
}
