using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

public record ReadUserQuery(Guid? Id, string? UniqueName, CustomIdentifierModel? Identifier) : Activity, IRequest<UserModel?>;

internal class ReadUserQueryHandler : IRequestHandler<ReadUserQuery, UserModel?>
{
  private readonly IUserQuerier _userQuerier;

  public ReadUserQueryHandler(IUserQuerier userQuerier)
  {
    _userQuerier = userQuerier;
  }

  public async Task<UserModel?> Handle(ReadUserQuery query, CancellationToken cancellationToken)
  {
    RealmModel? realm = query.Realm;

    Dictionary<Guid, UserModel> users = new(capacity: 3);

    if (query.Id.HasValue)
    {
      UserModel? user = await _userQuerier.ReadAsync(realm, query.Id.Value, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      UserModel? user = await _userQuerier.ReadAsync(realm, query.UniqueName, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
      else if (query.RequireUniqueEmail)
      {
        EmailModel email = new(query.UniqueName);
        IEnumerable<UserModel> usersByEmail = await _userQuerier.ReadAsync(realm, email, cancellationToken);
        if (usersByEmail.Count() == 1)
        {
          user = usersByEmail.Single();
          users[user.Id] = user;
        }
      }
    }

    if (query.Identifier != null)
    {
      UserModel? user = await _userQuerier.ReadAsync(realm, query.Identifier, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (users.Count > 1)
    {
      throw new TooManyResultsException<UserModel>(expectedCount: 1, actualCount: users.Count);
    }

    return users.Values.SingleOrDefault();
  }
}
