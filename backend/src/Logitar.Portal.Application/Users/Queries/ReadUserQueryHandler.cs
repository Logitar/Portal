using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
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
    Realm? realm = query.Realm;

    Dictionary<Guid, User> users = new(capacity: 3);

    if (query.Id.HasValue)
    {
      User? user = await _userQuerier.ReadAsync(realm, query.Id.Value, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      User? user = await _userQuerier.ReadAsync(realm, query.UniqueName, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
      else if (query.RequireUniqueEmail)
      {
        Email email = new(query.UniqueName);
        IEnumerable<User> usersByEmail = await _userQuerier.ReadAsync(realm, email, cancellationToken);
        if (usersByEmail.Count() == 1)
        {
          user = usersByEmail.Single();
          users[user.Id] = user;
        }
      }
    }

    if (query.Identifier != null)
    {
      User? user = await _userQuerier.ReadAsync(realm, query.Identifier, cancellationToken);
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
