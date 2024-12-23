﻿using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Realms.Events;

public record RealmDeleted : DomainEvent, IDeleteEvent, INotification;
