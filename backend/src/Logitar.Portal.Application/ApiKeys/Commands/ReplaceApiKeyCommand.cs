﻿using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.ApiKeys;
using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands;

internal record ReplaceApiKeyCommand(Guid Id, ReplaceApiKeyPayload Payload, long? Version) : Activity, IRequest<ApiKey?>;
