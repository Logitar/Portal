using MediatR;

namespace Logitar.Portal.Application.Pipeline;

internal class RequestPipeline : IRequestPipeline
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;

  public RequestPipeline(IApplicationContext applicationContext, IMediator mediator)
  {
    _applicationContext = applicationContext;
    _mediator = mediator;
  }

  public async Task<T> ExecuteAsync<T>(IRequest<T> request, CancellationToken cancellationToken)
  {
    if (request is ApplicationRequest applicationRequest)
    {
      _applicationContext.Contextualize(applicationRequest);
    }

    return await _mediator.Send(request, cancellationToken);
  }
}
