using Logitar.Portal.Application;
using MediatR;

namespace Logitar.Portal.Infrastructure
{
  internal class RequestPipeline : IRequestPipeline
  {
    private readonly ILoggingContext _log;
    private readonly IMediator _mediator;

    public RequestPipeline(ILoggingContext log, IMediator mediator)
    {
      _log = log;
      _mediator = mediator;
    }

    public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
    {
      _log.SetRequest(request);

      try
      {
        return await _mediator.Send(request, cancellationToken);
      }
      catch (Exception exception)
      {
        _log.AddError(exception);

        throw;
      }
    }
  }
}
