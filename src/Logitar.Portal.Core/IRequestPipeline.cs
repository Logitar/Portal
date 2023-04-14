using MediatR;

namespace Logitar.Portal.Core;

internal interface IRequestPipeline
{
  Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
}
