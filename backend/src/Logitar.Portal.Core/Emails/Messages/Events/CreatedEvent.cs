using Logitar.Portal.Core.Emails.Senders;

namespace Logitar.Portal.Core.Emails.Messages.Events
{
  public class CreatedEvent : CreatedEventBase
  {
    public CreatedEvent(string subject, string body, IEnumerable<Recipient> recipients,
      Guid? realmId, string? realmAlias, string? realmName,
      Guid senderId, bool senderIsDefault, ProviderType senderProvider, string senderAddress, string? senderDisplayName,
      Guid templateId, string templateKey, string templateContentType, string? templateDisplayName,
      bool ignoreUserLocale, string? locale, Dictionary<string, string?>? variables, bool isDemo, Guid userId) : base(userId)
    {
      Body = body ?? throw new ArgumentNullException(nameof(body));
      IgnoreUserLocale = ignoreUserLocale;
      IsDemo = isDemo;
      Locale = locale;
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
      TemplateContentType = templateContentType ?? throw new ArgumentNullException(nameof(templateContentType));
      TemplateDisplayName = templateDisplayName;
      TemplateId = templateId;
      TemplateKey = templateKey ?? throw new ArgumentNullException(nameof(templateKey));
      Variables = variables;
    }

    public string Body { get; private set; }
    public bool IgnoreUserLocale { get; private set; }
    public bool IsDemo { get; private set; }
    public string? Locale { get; set; }
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
    public string TemplateContentType { get; private set; }
    public string? TemplateDisplayName { get; private set; }
    public Guid TemplateId { get; private set; }
    public string TemplateKey { get; private set; }
    public Dictionary<string, string?>? Variables { get; private set; }
  }
}
