﻿using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderDeletedEvent : DomainEvent, INotification
{
  public SenderDeletedEvent()
  {
    IsDeleted = true;
  }
}
