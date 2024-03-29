﻿using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Queries;

internal record ReadConfigurationQuery : Activity, IRequest<Configuration>;
