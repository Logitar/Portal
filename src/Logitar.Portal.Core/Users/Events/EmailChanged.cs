﻿using Logitar.EventSourcing;
using Logitar.Portal.Core.Users.Contact;
using MediatR;

namespace Logitar.Portal.Core.Users.Events;

public record EmailChanged : DomainEvent, INotification
{
  public ReadOnlyEmail? Email { get; init; }
  public VerificationAction? VerificationAction { get; init; }
}
