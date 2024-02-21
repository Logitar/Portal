using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Messages.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class MessageEntity : AggregateEntity
{
  public int MessageId { get; private set; }

  public string? TenantId { get; private set; }

  public string Subject { get; private set; } = string.Empty;
  public string BodyType { get; private set; } = string.Empty;
  public string BodyText { get; private set; } = string.Empty;

  public int RecipientCount { get; private set; }
  public List<RecipientEntity> Recipients { get; private set; } = [];

  public SenderEntity? Sender { get; private set; }
  public int? SenderId { get; private set; }
  public bool SenderIsDefault { get; private set; }
  public string SenderAddress { get; private set; } = string.Empty;
  public string? SenderDisplayName { get; private set; }
  public SenderProvider SenderProvider { get; private set; }

  public TemplateEntity? Template { get; private set; }
  public int? TemplateId { get; private set; }
  public string TemplateUniqueKey { get; private set; } = string.Empty;
  public string? TemplateDisplayName { get; private set; }

  public bool IgnoreUserLocale { get; private set; }
  public string? Locale { get; private set; }

  public Dictionary<string, string> Variables { get; private set; } = [];
  public string? VariablesSerialized
  {
    get => Variables.Count == 0 ? null : JsonSerializer.Serialize(Variables);
    private set
    {
      if (value == null)
      {
        Variables.Clear();
      }
      else
      {
        Variables = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? [];
      }
    }
  }

  public bool IsDemo { get; private set; }

  public MessageStatus Status { get; private set; }
  public Dictionary<string, string> ResultData { get; private set; } = [];
  public string? ResultDataSerialized
  {
    get => ResultData.Count == 0 ? null : JsonSerializer.Serialize(ResultData);
    private set
    {
      if (value == null)
      {
        ResultData.Clear();
      }
      else
      {
        ResultData = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? [];
      }
    }
  }

  public MessageEntity(SenderEntity sender, TemplateEntity template, Dictionary<string, UserEntity> users, MessageCreatedEvent @event) : base(@event)
  {
    TenantId = @event.TenantId?.Value;

    Subject = @event.Subject.Value;
    BodyType = @event.Body.Type;
    BodyText = @event.Body.Text;

    foreach (RecipientUnit recipient in @event.Recipients)
    {
      UserEntity? user = null;
      if (recipient.UserId != null && !users.TryGetValue(recipient.UserId.Value, out user))
      {
        throw new InvalidOperationException($"The user entity 'AggregateId={recipient.UserId.Value}' could not be found.");
      }

      Recipients.Add(new RecipientEntity(this, user, recipient));
    }
    RecipientCount = Recipients.Count;

    Sender = sender;
    SenderId = sender.SenderId;
    SenderIsDefault = @event.Sender.IsDefault;
    SenderAddress = @event.Sender.Email.Address;
    SenderDisplayName = @event.Sender.DisplayName?.Value;
    SenderProvider = @event.Sender.Provider;

    Template = template;
    TemplateId = template.TemplateId;
    TemplateUniqueKey = @event.Template.UniqueKey.Value;
    TemplateDisplayName = @event.Template.DisplayName?.Value;

    IgnoreUserLocale = @event.IgnoreUserLocale;
    Locale = @event.Locale?.Code;

    foreach (KeyValuePair<string, string> variable in @event.Variables)
    {
      Variables[variable.Key] = variable.Value;
    }

    IsDemo = @event.IsDemo;

    Status = MessageStatus.Unsent;
  }

  private MessageEntity() : base()
  {
  }

  public void Fail(MessageFailedEvent @event)
  {
    Update(@event);

    Status = MessageStatus.Failed;
    SetResultData(@event.ResultData);
  }

  public void Succeed(MessageSucceededEvent @event)
  {
    Update(@event);

    Status = MessageStatus.Succeeded;
    SetResultData(@event.ResultData);
  }

  private void SetResultData(IReadOnlyDictionary<string, string> resultData)
  {
    foreach (KeyValuePair<string, string> data in resultData)
    {
      ResultData[data.Key] = data.Value;
    }
  }
}
