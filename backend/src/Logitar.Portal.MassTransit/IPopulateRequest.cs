using MassTransit;
using MediatR;

namespace Logitar.Portal.MassTransit;

internal interface IPopulateRequest
{
  Task ExecuteAsync<T>(ConsumeContext context, IRequest<T> request);
}
