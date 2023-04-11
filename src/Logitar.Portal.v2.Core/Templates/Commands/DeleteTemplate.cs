﻿using Logitar.Portal.v2.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.v2.Core.Templates.Commands;

internal record DeleteTemplate(Guid Id) : IRequest<Template>;