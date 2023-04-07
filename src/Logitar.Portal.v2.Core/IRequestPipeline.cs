using MediatR;

namespace Logitar.Portal.v2.Core;

internal interface IRequestPipeline
{
  Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
}
