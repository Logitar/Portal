﻿using MediatR;

namespace Logitar.Portal.Application.Caching.Commands;

public record InitializeCacheCommand : IRequest<Unit>;