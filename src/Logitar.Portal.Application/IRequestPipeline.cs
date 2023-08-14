using MediatR;

namespace Logitar.Portal.Application;

internal interface IRequestPipeline
{
  Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
}
