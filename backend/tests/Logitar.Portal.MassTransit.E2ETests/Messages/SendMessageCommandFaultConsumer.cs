using Logitar.Portal.Contracts.Messages;
using MassTransit;

namespace Logitar.Portal.MassTransit.Messages;

internal class SendMessageCommandFaultConsumer : IConsumer<Fault<SendMessageCommand>>
{
  public Task Consume(ConsumeContext<Fault<SendMessageCommand>> context)
  {
    foreach (ExceptionInfo exception in context.Message.Exceptions)
    {
      Console.WriteLine("ERROR: {0}: {1}", exception.ExceptionType, exception.Message);
    }

    return Task.CompletedTask;
  }
}
