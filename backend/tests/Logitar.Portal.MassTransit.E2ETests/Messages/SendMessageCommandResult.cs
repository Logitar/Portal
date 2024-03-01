using Logitar.Portal.Contracts.Messages;
using MassTransit;

namespace Logitar.Portal.MassTransit.Messages;

internal record SendMessageCommandResult
{
  public IReadOnlyCollection<ExceptionInfo>? Exceptions { get; private set; }
  public SentMessages? SentMessages { get; private set; }

  public bool HasSucceeded => SentMessages != null;

  private SendMessageCommandResult()
  {
  }

  public static SendMessageCommandResult Fail(IReadOnlyCollection<ExceptionInfo> exceptions) => new()
  {
    Exceptions = exceptions
  };
  public static SendMessageCommandResult Succeed(SentMessages sentMessages) => new()
  {
    SentMessages = sentMessages
  };
}
