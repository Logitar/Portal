using Logitar.EventSourcing.Infrastructure;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Messages.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record MessageEntity : AggregateEntity
{
  private static readonly JsonSerializerOptions _serializerOptions = new();
  static MessageEntity()
  {
    _serializerOptions.Converters.Add(new AggregateIdConverter());
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
  }

  public MessageEntity(MessageCreatedEvent created, RealmEntity? realm, SenderEntity sender,
    TemplateEntity template, IEnumerable<UserEntity> users) : base(created)
  {
    Subject = created.Subject;
    Body = created.Body;

    Dictionary<string, UserEntity> usersById = users.ToDictionary(u => u.AggregateId, u => u);
    foreach (ReadOnlyRecipient recipient in created.Recipients)
    {
      UserEntity? user = null;
      if (recipient.UserId.HasValue)
      {
        _ = usersById.TryGetValue(recipient.UserId.Value.Value, out user);
      }

      Recipients.Add(new RecipientEntity(recipient, this, user));
    }
    RecipientCount = Recipients.Count;

    Realm = realm;
    RealmId = realm?.RealmId;

    Sender = sender;
    SenderId = sender.SenderId;
    SenderSummary = created.Sender;

    Template = template;
    TemplateId = template.TemplateId;
    TemplateSummary = created.Template;

    IgnoreUserLocale = created.IgnoreUserLocale;
    Locale = created.Locale?.Code;

    foreach (KeyValuePair<string, string> variable in created.Variables)
    {
      Variables[variable.Key] = variable.Value;
    }

    IsDemo = created.IsDemo;

    Status = MessageStatus.Unsent;
  }

  private MessageEntity() : base()
  {
  }

  public int MessageId { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string Body { get; private set; } = string.Empty;

  public List<RecipientEntity> Recipients { get; private set; } = new();
  public int RecipientCount { get; private set; }

  public RealmEntity? Realm { get; private set; }
  public int? RealmId { get; private set; }

  public SenderEntity? Sender { get; private set; }
  public int? SenderId { get; private set; }
  public SenderSummary SenderSummary { get; private set; } = new();
  public string SenderSummarySerialized
  {
    get => JsonSerializer.Serialize(SenderSummary, _serializerOptions);
    private set => SenderSummary = JsonSerializer.Deserialize<SenderSummary>(value, _serializerOptions) ?? new();
  }

  public TemplateEntity? Template { get; private set; }
  public int? TemplateId { get; private set; }
  public TemplateSummary TemplateSummary { get; private set; } = new();
  public string TemplateSummarySerialized
  {
    get => JsonSerializer.Serialize(TemplateSummary, _serializerOptions);
    private set => TemplateSummary = JsonSerializer.Deserialize<TemplateSummary>(value, _serializerOptions) ?? new();
  }

  public bool IgnoreUserLocale { get; private set; }
  public string? Locale { get; private set; }

  public Dictionary<string, string> Variables { get; private set; } = new();
  public string? VariablesSerialized
  {
    get => Variables.Any() ? JsonSerializer.Serialize(Variables) : null;
    private set
    {
      if (value == null)
      {
        Variables.Clear();
      }
      else
      {
        Variables = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new();
      }
    }
  }

  public bool IsDemo { get; private set; }

  public Dictionary<string, string> Result { get; private set; } = new();
  public string? ResultSerialized
  {
    get => Result.Any() ? JsonSerializer.Serialize(Result) : null;
    private set
    {
      if (value == null)
      {
        Result.Clear();
      }
      else
      {
        Result = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new();
      }
    }
  }
  public MessageStatus Status { get; private set; }

  public void Succeed(MessageSucceededEvent succeeded)
  {
    Update(succeeded);

    foreach (KeyValuePair<string, string> data in succeeded.Result)
    {
      Result[data.Key] = data.Value;
    }

    Status = MessageStatus.Succeeded;
  }

  public void Fail(MessageFailedEvent failed)
  {
    Update(failed);

    foreach (KeyValuePair<string, string> data in failed.Result)
    {
      Result[data.Key] = data.Value;
    }

    Status = MessageStatus.Failed;
  }
}
