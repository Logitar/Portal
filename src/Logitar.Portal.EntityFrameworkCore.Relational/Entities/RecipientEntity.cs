using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record RecipientEntity
{
  public RecipientEntity(ReadOnlyRecipient recipient, MessageEntity message, UserEntity? user)
  {
    Message = message;
    MessageId = message.MessageId;

    Type = recipient.Type;

    User = user;
    UserId = user?.UserId;
    UserSummary = user == null ? null : UserSummary.From(user);

    Address = recipient.Address;
    DisplayName = recipient.DisplayName;
  }

  private RecipientEntity()
  {
  }

  public long RecipientId { get; private set; }

  public MessageEntity? Message { get; private set; }
  public int MessageId { get; private set; }

  public RecipientType Type { get; private set; }

  public string Address { get; private set; } = string.Empty;
  public string? DisplayName { get; private set; }

  public UserEntity? User { get; private set; }
  public int? UserId { get; private set; }
  public UserSummary? UserSummary { get; private set; }
  public string? UserSummarySerialized
  {
    get => UserSummary == null ? null : JsonSerializer.Serialize(UserSummary);
    private set => UserSummary = value == null ? null : JsonSerializer.Deserialize<UserSummary>(value);
  }
}
