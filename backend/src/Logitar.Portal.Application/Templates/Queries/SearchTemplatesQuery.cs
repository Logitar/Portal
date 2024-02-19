﻿using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Templates;
using MediatR;

namespace Logitar.Portal.Application.Templates.Queries;

internal record SearchTemplatesQuery(SearchTemplatesPayload Payload) : IRequest<SearchResults<Template>>;