using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal class ReadUserQueryHandler : IRequestHandler<ReadUserQuery, User?>
{
  private readonly IUserQuerier _userQuerier;

  public ReadUserQueryHandler(IUserQuerier userQuerier)
  {
    _userQuerier = userQuerier;
  }

  public async Task<User?> Handle(ReadUserQuery query, CancellationToken cancellationToken)
  {
    Dictionary<string, User> users = new(capacity: 2);

    if (query.Id != null)
    {
      User? user = await _userQuerier.ReadAsync(query.Id, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (query.UniqueName != null)
    {
      User? user = await _userQuerier.ReadAsync(query.Realm, query.UniqueName, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (users.Count > 1)
    {
      throw new TooManyResultsException<User>(expected: 1, actual: users.Count);
    }

    return users.Values.SingleOrDefault();
  }
}
