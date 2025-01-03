﻿using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateDeleted : DomainEvent, IDeleteEvent, INotification;
