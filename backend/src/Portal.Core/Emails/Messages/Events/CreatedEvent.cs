using Portal.Core.Emails.Senders;

namespace Portal.Core.Emails.Messages.Events
{
  public class CreatedEvent : CreatedEventBase
  {
    public CreatedEvent(string subject, string body, string contentType,
      IEnumerable<Recipient> recipients,
      Guid? realmId, string? realmAlias, string? realmName,
      Guid senderId, bool senderIsDefault, ProviderType senderProvider, string senderAddress, string? senderDisplayName,
      Guid templateId, string templateKey, string? templateDisplayName,
      Dictionary<string, string?>? variables, Guid userId) : base(userId)
    {
      Body = body ?? throw new ArgumentNullException(nameof(body));
      ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
      RealmAlias = realmAlias;
      RealmId = realmId;
      RealmName = realmName;
      Recipients = recipients ?? throw new ArgumentNullException(nameof(recipients));
      SenderAddress = senderAddress ?? throw new ArgumentNullException(nameof(senderAddress));
      SenderDisplayName = senderDisplayName;
      SenderId = senderId;
      SenderIsDefault = senderIsDefault;
      SenderProvider = senderProvider;
      Subject = subject ?? throw new ArgumentNullException(nameof(subject));
      TemplateDisplayName = templateDisplayName;
      TemplateId = templateId;
      TemplateKey = templateKey ?? throw new ArgumentNullException(nameof(templateKey));
      Variables = variables;
    }

    public string Body { get; private set; }
    public string ContentType { get; private set; }
    public string? RealmAlias { get; private set; }
    public Guid? RealmId { get; private set; }
    public string? RealmName { get; private set; }
    public IEnumerable<Recipient> Recipients { get; private set; }
    public string SenderAddress { get; private set; }
    public string? SenderDisplayName { get; private set; }
    public Guid SenderId { get; private set; }
    public bool SenderIsDefault { get; private set; }
    public ProviderType SenderProvider { get; private set; }
    public string Subject { get; private set; }
    public string? TemplateDisplayName { get; private set; }
    public Guid TemplateId { get; private set; }
    public string TemplateKey { get; private set; }
    public Dictionary<string, string?>? Variables { get; private set; }
  }
}
