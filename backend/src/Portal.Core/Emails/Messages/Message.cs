using Portal.Core.Emails.Messages.Events;
using Portal.Core.Emails.Senders;
using Portal.Core.Emails.Templates;
using Portal.Core.Realms;
using System.Text.Json;

namespace Portal.Core.Emails.Messages
{
  public class Message : Aggregate
  {
    public Message(string subject, string body, IEnumerable<Recipient> recipients, Sender sender,
      Template template, Guid userId, Realm? realm = null, Dictionary<string, string?>? variables = null)
    {
      ArgumentNullException.ThrowIfNull(sender);
      ArgumentNullException.ThrowIfNull(template);

      ApplyChange(new CreatedEvent(subject, body, recipients,
        realm?.Id, realm?.Alias, realm?.Name,
        sender.Id, sender.IsDefault, sender.Provider, sender.EmailAddress, sender.DisplayName,
        template.Id, template.Key, template.ContentType, template.DisplayName,
        variables, userId));
    }
    private Message()
    {
    }

    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;

    public IEnumerable<Recipient> Recipients { get; private set; } = null!;
    public string RecipientsSerialized
    {
      get => JsonSerializer.Serialize(Recipients);
      private set => Recipients = JsonSerializer.Deserialize<IEnumerable<Recipient>>(value) ?? Enumerable.Empty<Recipient>();
    }

    public Guid SenderId { get; private set; }
    public bool SenderIsDefault { get; private set; }
    public ProviderType SenderProvider { get; private set; }
    public string SenderAddress { get; private set; } = null!;
    public string? SenderDisplayName { get; private set; }

    public Guid? RealmId { get; private set; }
    public string? RealmAlias { get; private set; }
    public string? RealmName { get; private set; }

    public Guid TemplateId { get; private set; }
    public string TemplateKey { get; private set; } = null!;
    public string TemplateContentType { get; private set; } = null!;
    public string? TemplateDisplayName { get; private set; }

    protected virtual void Apply(CreatedEvent @event)
    {
      Subject = @event.Subject.Trim();
      Body = @event.Body.Trim();

      Recipients = @event.Recipients;

      SenderId = @event.SenderId;
      SenderIsDefault = @event.SenderIsDefault;
      SenderProvider = @event.SenderProvider;
      SenderAddress = @event.SenderAddress;
      SenderDisplayName = @event.SenderDisplayName;

      RealmId = @event.RealmId;
      RealmAlias = @event.RealmAlias;
      RealmName = @event.RealmName;

      TemplateId = @event.TemplateId;
      TemplateKey = @event.TemplateKey;
      TemplateContentType = @event.TemplateContentType;
      TemplateDisplayName = @event.TemplateDisplayName;
    }

    public override string ToString() => $"{Subject} | {base.ToString()}";
  }
}
