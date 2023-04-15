using Logitar.Portal.Domain.Sessions.Events;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Domain.Sessions
{
  public class Session : Aggregate
  {
    public Session(User user, string? keyHash = null, string? ipAddress = null, string? additionalInformation = null)
    {
      User = user ?? throw new ArgumentNullException(nameof(user));
      UserSid = user.Sid;

      ApplyChange(new CreatedEvent(user.Id, keyHash, ipAddress, additionalInformation));
    }
    private Session()
    {
    }

    public User? User { get; private set; }
    public int UserSid { get; private set; }

    public string? KeyHash { get; private set; }
    public bool IsPersistent
    {
      get => KeyHash != null;
      private set { /* EntityFrameworkCore only setter */ }
    }

    public DateTime? SignedOutAt { get; private set; }
    public Guid? SignedOutById { get; private set; }
    public bool IsActive
    {
      get => !SignedOutAt.HasValue && !SignedOutById.HasValue;
      private set { /* EntityFrameworkCore only setter */ }
    }

    public string? IpAddress { get; private set; }
    public string? AdditionalInformation { get; private set; }

    private Guid UserId => User?.Id ?? throw new InvalidOperationException($"The {nameof(User)} is required.");

    public void Delete(Guid userId) => ApplyChange(new DeletedEvent(userId));
    public void Update(string? keyHash = null, string? ipAddress = null, string? additionalInformation = null) => ApplyChange(new UpdatedEvent(UserId, keyHash, ipAddress, additionalInformation));

    public void SignOut(Guid? userId = null) => ApplyChange(new SignedOutEvent(userId ?? UserId));

    protected virtual void Apply(CreatedEvent @event)
    {
      KeyHash = @event.KeyHash;

      IpAddress = @event.IpAddress;
      AdditionalInformation = @event.AdditionalInformation;
    }
    protected virtual void Apply(DeletedEvent @event)
    {
    }
    protected virtual void Apply(SignedOutEvent @event)
    {
      SignedOutAt = @event.OccurredAt;
      SignedOutById = @event.UserId;
    }
    protected virtual void Apply(UpdatedEvent @event)
    {
      KeyHash = @event.KeyHash;

      IpAddress = @event.IpAddress;
      AdditionalInformation = @event.AdditionalInformation;
    }
  }
}
