﻿using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageFailedEvent : DomainEvent, INotification
{
  public IReadOnlyDictionary<string, string> ResultData { get; }

  public MessageFailedEvent(ActorId actorId, IReadOnlyDictionary<string, string> resultData)
  {
    ActorId = actorId;
    ResultData = resultData;
  }
}
