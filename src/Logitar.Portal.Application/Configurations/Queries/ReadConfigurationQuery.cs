﻿using MediatR;

namespace Logitar.Portal.Application.Configurations.Queries;

public record ReadConfigurationQuery : IRequest<Configuration>;
