﻿using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries
{
  internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserModel?>
  {
    private readonly IUserQuerier _userQuerier;

    public GetUserQueryHandler(IUserQuerier userQuerier)
    {
      _userQuerier = userQuerier;
    }

    public async Task<UserModel?> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
      return await _userQuerier.GetAsync(request.Id, cancellationToken);
    }
  }
}