﻿using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmTemplatesCommand(Realm Realm, ActorId ActorId) : INotification;
