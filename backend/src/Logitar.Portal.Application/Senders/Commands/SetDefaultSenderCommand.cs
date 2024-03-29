﻿using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal record SetDefaultSenderCommand(Guid Id) : Activity, IRequest<Sender?>;
