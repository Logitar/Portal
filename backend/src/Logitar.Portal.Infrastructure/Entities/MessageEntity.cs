using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Messages.Events;
using System.Globalization;
using System.Text.Json;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class MessageEntity : AggregateEntity
  {
    public MessageEntity(MessageCreatedEvent @event, Actor actor) : base(@event, actor)
    {
      Subject = @event.Subject;
      Body = @event.Body;

      foreach (Recipient recipient in @event.Recipients)
      {
        Recipients.Add(new RecipientEntity(recipient, this));
      }
      RecipientCount = Recipients.Count;

      SenderId = @event.SenderId.Value;
      SenderIsDefault = @event.SenderIsDefault;
      SenderAddress = @event.SenderAddress;
      SenderDisplayName = @event.SenderDisplayName;
      SenderProvider = @event.SenderProvider;

      TemplateId = @event.TemplateId.Value;
      TemplateKey = @event.TemplateKey;
      TemplateDisplayName = @event.TemplateDisplayName;
      TemplateContentType = @event.TemplateContentType;

      RealmId = @event.RealmId?.Value;
      RealmAlias = @event.RealmAlias;
      RealmDisplayName = @event.RealmDisplayName;

      IgnoreUserLocale = @event.IgnoreUserLocale;
      Locale = @event.Locale;

      Variables = @event.Variables?.Any() == true ? JsonSerializer.Serialize(@event.Variables) : null;

      IsDemo = @event.IsDemo;
    }
    private MessageEntity() : base()
    {
    }

    public long MessageId { get; private set; }

    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;

    public List<RecipientEntity> Recipients { get; private set; } = new();
    public int RecipientCount { get; private set; }

    public string SenderId { get; private set; } = string.Empty;
    public bool SenderIsDefault { get; private set; }
    public string SenderAddress { get; private set; } = string.Empty;
    public string? SenderDisplayName { get; private set; }
    public ProviderType SenderProvider { get; private set; }

    public string TemplateId { get; private set; } = string.Empty;
    public string TemplateKey { get; private set; } = string.Empty;
    public string TemplateKeyNormalized
    {
      get => TemplateKey.ToUpper();
      private set { }
    }
    public string? TemplateDisplayName { get; private set; }
    public string TemplateContentType { get; private set; } = string.Empty;

    public string? RealmId { get; private set; }
    public string? RealmAlias { get; private set; }
    public string? RealmAliasNormalized
    {
      get => RealmAlias?.ToUpper();
      private set { }
    }
    public string? RealmDisplayName { get; private set; }

    public bool IgnoreUserLocale { get; private set; }
    public CultureInfo? Locale { get; private set; }

    public string? Variables { get; private set; }

    public bool IsDemo { get; private set; }

    public string? Errors { get; private set; }
    public bool HasErrors
    {
      get => Errors != null;
      private set { }
    }

    public string? Result { get; private set; }
    public bool HasSucceeded
    {
      get => !HasErrors && Result != null;
      private set { }
    }

    public void Fail(MessageFailedEvent @event, Actor actor)
    {
      Update(@event, actor);

      Errors = JsonSerializer.Serialize(@event.Errors);
    }
    public void Succeed(MessageSucceededEvent @event, Actor actor)
    {
      Update(@event, actor);

      Result = JsonSerializer.Serialize(@event.Result);
    }
  }
}
