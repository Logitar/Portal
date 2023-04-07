using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Users;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Queries;

internal class GetUserHandler : IRequestHandler<GetUser, User?>
{
  private readonly IUserQuerier _userQuerier;

  public GetUserHandler(IUserQuerier userQuerier)
  {
    _userQuerier = userQuerier;
  }

  public async Task<User?> Handle(GetUser request, CancellationToken cancellationToken)
  {
    List<User> users = new(capacity: 3);

    if (request.Id.HasValue)
    {
      users.AddIfNotNull(await _userQuerier.GetAsync(request.Id.Value, cancellationToken));
    }
    if (request.Username != null)
    {
      users.AddIfNotNull(await _userQuerier.GetAsync(request.Realm, request.Username, cancellationToken));
    }
    if (request.ExternalKey != null && request.ExternalValue != null)
    {
      users.AddIfNotNull(await _userQuerier.GetAsync(request.Realm, request.ExternalKey, request.ExternalValue, cancellationToken));
    }

    if (users.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return users.SingleOrDefault();
  }
}
