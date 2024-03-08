using MediatR;

namespace Logitar.Portal.Application.Activities;

public interface IActivityPipeline
{
  Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
  Task<T> ExecuteAsync<T>(IRequest<T> request, IContextParameters parameters, CancellationToken cancellationToken = default);
}
