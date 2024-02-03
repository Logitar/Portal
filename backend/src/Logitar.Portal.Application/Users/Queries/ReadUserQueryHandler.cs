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
    Dictionary<Guid, User> users = new(capacity: 3);

    if (query.Id.HasValue)
    {
      User? user = await _userQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      User? user = await _userQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (query.Identifier != null)
    {
      User? user = await _userQuerier.ReadAsync(query.Identifier, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (users.Count > 1)
    {
      throw new TooManyResultsException<User>(expectedCount: 1, actualCount: users.Count);
    }

    return users.Values.SingleOrDefault();
  }
}
