using MediatR;

namespace Logitar.Portal.Application;

public interface IActivityPipeline // TODO(fpion): move to Activities namespace with all activity classes & interfaces
{
  Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken = default);
}
