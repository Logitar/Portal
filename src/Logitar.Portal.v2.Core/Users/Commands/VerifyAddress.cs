﻿using Logitar.Portal.v2.Contracts.Users;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Commands;

internal record VerifyAddress(Guid Id) : IRequest<User>;
