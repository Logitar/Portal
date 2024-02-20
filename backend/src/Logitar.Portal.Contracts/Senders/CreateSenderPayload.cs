﻿using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Senders;

public record CreateSenderPayload
{
  public EmailPayload Email { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public SendGridSettings SendGrid { get; set; } = new();

  public CreateSenderPayload() : this(new EmailPayload())
  {
  }

  public CreateSenderPayload(EmailPayload email)
  {
    Email = email;
  }
}
