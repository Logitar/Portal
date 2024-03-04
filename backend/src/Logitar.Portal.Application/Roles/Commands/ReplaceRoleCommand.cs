﻿using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record ReplaceRoleCommand(Guid Id, ReplaceRolePayload Payload, long? Version) : ApplicationRequest, IRequest<Role?>;
