﻿using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal record UpdateConfigurationCommand(UpdateConfigurationPayload Payload) : Activity, IRequest<Configuration>;
