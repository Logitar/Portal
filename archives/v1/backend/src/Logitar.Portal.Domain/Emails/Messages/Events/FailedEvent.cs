using Logitar.Portal.Core;

namespace Logitar.Portal.Domain.Emails.Messages.Events
{
  public class FailedEvent : UpdatedEventBase
  {
    public FailedEvent(IEnumerable<Error> errors, Guid userId) : base(userId)
    {
      Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }

    public IEnumerable<Error> Errors { get; private set; }
  }
}
