﻿using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record ResetPasswordCommand(ResetPasswordPayload Payload) : IRequest<User>;