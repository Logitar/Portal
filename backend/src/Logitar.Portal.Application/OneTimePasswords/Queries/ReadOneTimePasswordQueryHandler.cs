﻿using Logitar.Portal.Contracts.Passwords;
using MediatR;

namespace Logitar.Portal.Application.OneTimePasswords.Queries;

internal class ReadOneTimePasswordQueryHandler : IRequestHandler<ReadOneTimePasswordQuery, OneTimePasswordModel?>
{
  private readonly IOneTimePasswordQuerier _oneTimePasswordQuerier;

  public ReadOneTimePasswordQueryHandler(IOneTimePasswordQuerier oneTimePasswordQuerier)
  {
    _oneTimePasswordQuerier = oneTimePasswordQuerier;
  }

  public async Task<OneTimePasswordModel?> Handle(ReadOneTimePasswordQuery query, CancellationToken cancellationToken)
  {
    return await _oneTimePasswordQuerier.ReadAsync(query.Realm, query.Id, cancellationToken);
  }
}
