namespace Logitar.Portal.Core.Emails.Messages.Events
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
