﻿using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal record UpdateDictionaryCommand(Guid Id, UpdateDictionaryPayload Payload) : IRequest<Dictionary?>;
