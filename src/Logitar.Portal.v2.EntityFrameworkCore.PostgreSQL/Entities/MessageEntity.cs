using Logitar.Portal.v2.Core.Messages.Events;
using System.Text.Json;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;

internal class MessageEntity : AggregateEntity
{
  public MessageEntity(MessageCreated e, ActorEntity actor) : base(e, actor)
  {
    IsDemo = e.IsDemo;

    Subject = e.Subject;
    Body = e.Body;

    Recipients = JsonSerializer.Serialize(e.Recipients);

    RealmId = e.Realm.Id.ToGuid();
    RealmUniqueName = e.Realm.UniqueName;
    RealmDisplayName = e.Realm.DisplayName;

    SenderId = e.Sender.Id.ToGuid();
    SenderProvider = e.Sender.Provider.ToString();
    SenderIsDefault = e.Sender.IsDefault;
    SenderEmailAddress = e.Sender.EmailAddress;
    SenderDisplayName = e.Sender.DisplayName;

    TemplateId = e.Template.Id.ToGuid();
    TemplateUniqueName = e.Template.UniqueName;
    TemplateDisplayName = e.Template.DisplayName;
    TemplateContentType = e.Template.ContentType;

    IgnoreUserLocale = e.IgnoreUserLocale;
    Locale = e.Locale?.Name;

    Variables = e.Variables.Any() ? JsonSerializer.Serialize(e.Variables) : null;
  }

  private MessageEntity() : base()
  {
  }

  public int MessageId { get; private set; }

  public bool IsDemo { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string Body { get; private set; } = string.Empty;

  public string Recipients { get; private set; } = string.Empty;

  public Guid RealmId { get; private set; }
  public string RealmUniqueName { get; private set; } = string.Empty;
  public string? RealmDisplayName { get; private set; }

  public Guid SenderId { get; private set; }
  public bool SenderIsDefault { get; private set; }
  public string SenderProvider { get; private set; } = string.Empty;
  public string SenderEmailAddress { get; private set; } = string.Empty;
  public string? SenderDisplayName { get; private set; }

  public Guid TemplateId { get; private set; }
  public string TemplateUniqueName { get; private set; } = string.Empty;
  public string? TemplateDisplayName { get; private set; }
  public string TemplateContentType { get; private set; } = string.Empty;

  public bool IgnoreUserLocale { get; private set; }
  public string? Locale { get; private set; }

  public string? Variables { get; private set; }

  public string? Errors { get; private set; }
  public bool HasErrors
  {
    get => Errors != null;
    private set { }
  }

  public string? Result { get; private set; }
  public bool Succeeded
  {
    get => !HasErrors && Result != null;
    private set { }
  }

  public void Fail(MessageFailed e, ActorEntity actor)
  {
    Update(e, actor);

    Errors = JsonSerializer.Serialize(e.Errors);
  }

  public void Succeed(MessageSucceeded e, ActorEntity actor)
  {
    Update(e, actor);

    Result = JsonSerializer.Serialize(e.Result);
  }
}
