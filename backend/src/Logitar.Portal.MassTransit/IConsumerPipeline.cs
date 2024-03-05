using MassTransit;
using MediatR;

namespace Logitar.Portal.MassTransit;

public interface IConsumerPipeline
{
  Task<T> ExecuteAsync<T>(IRequest<T> request, Type consumerType, ConsumeContext context);
}
