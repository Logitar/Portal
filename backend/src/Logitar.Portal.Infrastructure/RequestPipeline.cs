using Logitar.Portal.Application;
using MediatR;

namespace Logitar.Portal.Infrastructure
{
  internal class RequestPipeline : IRequestPipeline
  {
    private readonly IMediator _mediator;

    public RequestPipeline(IMediator mediator)
    {
      _mediator = mediator;
    }

    /// <summary>
    /// TODO(fpion): implement logging
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
    {
      return await _mediator.Send(request, cancellationToken);
    }
  }
}
