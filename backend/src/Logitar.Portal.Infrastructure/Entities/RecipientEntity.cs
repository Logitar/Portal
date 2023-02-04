using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Domain.Messages;
using System.Globalization;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class RecipientEntity
  {
    public RecipientEntity(Recipient recipient, MessageEntity message)
    {
      Id = Guid.NewGuid();

      Message = message;
      MessageId = message.MessageId;

      Type = recipient.Type;

      Address = recipient.Address;
      DisplayName = recipient.DisplayName;

      UserId = recipient.UserId?.Value;
      Username = recipient.Username;
      UserLocale = recipient.UserLocale;
    }
    private RecipientEntity()
    {
    }

    public long RecipientId { get; private set; }

    public Guid Id { get; private set; }

    public MessageEntity? Message { get; private set; }
    public long MessageId { get; private set; }

    public RecipientType Type { get; private set; }

    public string Address { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }

    public string? UserId { get; private set; }
    public string? Username { get; private set; }
    public CultureInfo? UserLocale { get; private set; }
  }
}
