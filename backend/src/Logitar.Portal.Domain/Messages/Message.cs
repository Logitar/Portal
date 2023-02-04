using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Messages.Events;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Templates;
using System.Globalization;

namespace Logitar.Portal.Domain.Messages
{
  public class Message : AggregateRoot
  {
    public Message(AggregateId actorId, string subject, string body, IEnumerable<Recipient> recipients,
      Sender sender, Template template, Realm? realm = null, bool ignoreUserLocale = false, CultureInfo? locale = null,
      Dictionary<string, string?>? variables = null, bool isDemo = false) : base()
    {
      ApplyChange(new MessageCreatedEvent
      {
        Subject = subject,
        Body = body,
        Recipients = recipients,
        SenderId = sender.Id,
        SenderIsDefault = sender.IsDefault,
        SenderAddress = sender.EmailAddress,
        SenderDisplayName = sender.DisplayName,
        SenderProvider = sender.Provider,
        TemplateId = template.Id,
        TemplateKey = template.Key,
        TemplateDisplayName = template.DisplayName,
        TemplateContentType = template.ContentType,
        RealmId = realm?.Id,
        RealmAlias = realm?.Alias,
        RealmDisplayName = realm?.DisplayName,
        IgnoreUserLocale = ignoreUserLocale,
        Locale = locale,
        Variables = variables,
        IsDemo = isDemo
      }, actorId);
    }
    private Message() : base()
    {
    }

    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public IEnumerable<Recipient> Recipients { get; private set; } = Enumerable.Empty<Recipient>();

    public AggregateId SenderId { get; private set; }
    public bool SenderIsDefault { get; private set; }
    public string SenderAddress { get; private set; } = string.Empty;
    public string? SenderDisplayName { get; private set; }
    public ProviderType SenderProvider { get; private set; }

    public AggregateId TemplateId { get; private set; }
    public string TemplateKey { get; private set; } = string.Empty;
    public string? TemplateDisplayName { get; private set; }
    public string TemplateContentType { get; private set; } = string.Empty;

    public AggregateId? RealmId { get; private set; }
    public string? RealmAlias { get; private set; }
    public string? RealmDisplayName { get; private set; }

    public bool IgnoreUserLocale { get; private set; }
    public CultureInfo? Locale { get; private set; }

    public Dictionary<string, string?>? Variables { get; private set; }

    public bool IsDemo { get; private set; }

    public IEnumerable<Error> Errors { get; private set; } = Enumerable.Empty<Error>();
    public bool HasErrors => Errors.Any();

    public SendMessageResult? Result { get; private set; }
    public bool HasSucceeded => !HasErrors && Result != null;

    public void Fail(AggregateId actorId, Error error)
    {
      if (HasErrors || HasSucceeded)
      {
        throw new InvalidOperationException($"The message 'Id={Id}' has already been sent.");
      }

      ApplyChange(new MessageFailedEvent
      {
        Errors = new[] { error }
      }, actorId);
    }
    public void Succeed(AggregateId actorId, SendMessageResult result)
    {
      if (HasErrors || HasSucceeded)
      {
        throw new InvalidOperationException($"The message 'Id={Id}' has already been sent.");
      }

      ApplyChange(new MessageSucceededEvent
      {
        Result = result
      }, actorId);
    }

    protected virtual void Apply(MessageCreatedEvent @event)
    {
      Subject = @event.Subject;
      Body = @event.Body;
      Recipients = @event.Recipients;

      SenderId = @event.SenderId;
      SenderIsDefault = @event.SenderIsDefault;
      SenderAddress = @event.SenderAddress;
      SenderDisplayName = @event.SenderDisplayName;
      SenderProvider = @event.SenderProvider;

      TemplateId = @event.TemplateId;
      TemplateKey = @event.TemplateKey;
      TemplateDisplayName = @event.TemplateDisplayName;
      TemplateContentType = @event.TemplateContentType;

      RealmId = @event.RealmId;
      RealmAlias = @event.RealmAlias;
      RealmDisplayName = @event.RealmDisplayName;

      IgnoreUserLocale = @event.IgnoreUserLocale;
      Locale = @event.Locale;

      Variables = @event.Variables;

      IsDemo = @event.IsDemo;
    }
    protected virtual void Apply(MessageFailedEvent @event)
    {
      Errors = @event.Errors;
    }
    protected virtual void Apply(MessageSucceededEvent @event)
    {
      Result = @event.Result;
    }
  }
}
