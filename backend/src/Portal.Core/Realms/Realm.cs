using Portal.Core.Emails.Senders;
using Portal.Core.Emails.Templates;
using Portal.Core.Realms.Events;
using Portal.Core.Realms.Payloads;
using Portal.Core.Users;

namespace Portal.Core.Realms
{
  public class Realm : Aggregate
  {
    public Realm(CreateRealmPayload payload, Guid userId)
    {
      ApplyChange(new CreatedEvent(payload, userId));
    }
    private Realm()
    {
    }

    public string Alias { get; private set; } = null!;
    public string AliasNormalized
    {
      get => Alias.ToUpper();
      private set { /* EntityFrameworkCore only setter */ }
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public bool RequireConfirmedAccount { get; private set; }
    public string? Url { get; private set; }

    public Sender? PasswordRecoverySender
    {
      get => PasswordRecoverySenderRelation?.Sender;
      set => PasswordRecoverySenderRelation = value == null ? null : new PasswordRecoverySender(this, value);
    }
    /// <summary>
    /// EntityFrameworkCore only property
    /// </summary>
    public PasswordRecoverySender? PasswordRecoverySenderRelation { get; private set; }
    public Template? PasswordRecoveryTemplate
    {
      get => PasswordRecoveryTemplateRelation?.Template;
      set => PasswordRecoveryTemplateRelation = value == null ? null : new PasswordRecoveryTemplate(this, value);
    }
    /// <summary>
    /// EntityFrameworkCore only property
    /// </summary>
    public PasswordRecoveryTemplate? PasswordRecoveryTemplateRelation { get; private set; }

    public List<User> Users { get; private set; } = new();

    public void Delete(Guid userId) => ApplyChange(new DeletedEvent(userId));
    public void Update(UpdateRealmPayload payload, Guid userId) => ApplyChange(new UpdatedEvent(payload, userId));

    protected virtual void Apply(CreatedEvent @event)
    {
      Alias = @event.Payload.Alias;

      Apply(@event.Payload);
    }
    protected virtual void Apply(DeletedEvent @event)
    {
    }
    protected virtual void Apply(UpdatedEvent @event)
    {
      Apply(@event.Payload);
    }

    private void Apply(SaveRealmPayload payload)
    {
      Name = payload.Name.Trim();
      Description = payload.Description?.CleanTrim();

      RequireConfirmedAccount = payload.RequireConfirmedAccount;
      Url = payload.Url;
    }

    public override string ToString() => $"{Name} | {base.ToString()}";
  }
}
