﻿using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal record CreateDictionaryCommand(CreateDictionaryPayload Payload) : Activity, IRequest<Dictionary>;
