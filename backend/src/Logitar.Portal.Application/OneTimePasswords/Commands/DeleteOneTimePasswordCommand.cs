﻿using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Commands;

internal record DeleteOneTimePasswordCommand(Guid Id) : IRequest<OneTimePassword?>;