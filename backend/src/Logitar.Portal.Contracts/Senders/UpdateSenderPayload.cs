using Logitar.Identity.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Senders;

public record UpdateSenderPayload
{
  public EmailPayload? Email { get; set; }
  public Modification<string>? DisplayName { get; set; }
  public Modification<string>? Description { get; set; }

  public SendGridSettings? SendGrid { get; set; }
}
