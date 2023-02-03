﻿using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries
{
  internal record GetTemplateQuery(string Id) : IRequest<TemplateModel?>;
}
