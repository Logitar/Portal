﻿using Logitar.EventSourcing;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmUsersCommand(RealmAggregate Realm, ActorId ActorId) : INotification;
