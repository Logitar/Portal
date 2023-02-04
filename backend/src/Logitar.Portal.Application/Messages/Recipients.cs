using Logitar.Portal.Domain.Messages;

namespace Logitar.Portal.Application.Messages
{
  internal class Recipients
  {
    public List<Recipient> To { get; init; } = new();
    public List<Recipient> CC { get; init; } = new();
    public List<Recipient> Bcc { get; init; } = new();
  }
}
