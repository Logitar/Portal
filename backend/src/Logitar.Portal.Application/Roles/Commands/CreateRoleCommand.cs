﻿using Logitar.Portal.Contracts.Roles;
using MediatR;

namespace Logitar.Portal.Application.Roles.Commands;

internal record CreateRoleCommand(CreateRolePayload Payload) : ApplicationRequest, IRequest<Role>;
