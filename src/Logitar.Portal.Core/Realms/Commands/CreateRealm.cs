﻿using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Core.Realms.Commands;

internal record CreateRealm(CreateRealmInput Input) : IRequest<Realm>;