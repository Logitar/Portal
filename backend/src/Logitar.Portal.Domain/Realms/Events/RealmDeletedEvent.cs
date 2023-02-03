﻿using MediatR;

namespace Logitar.Portal.Domain.Realms.Events
{
  public record RealmDeletedEvent : DomainEvent, INotification
  {
  }
}