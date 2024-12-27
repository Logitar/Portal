using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal record FindUserQuery(TenantId? TenantId, string User, IUserSettings UserSettings, string? PropertyName, bool IncludeId = false) : IRequest<User>;

internal class FindUserQueryHandler : IRequestHandler<FindUserQuery, User>
{
  private readonly IUserRepository _userRepository;

  public FindUserQueryHandler(IUserRepository userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<User> Handle(FindUserQuery query, CancellationToken cancellationToken)
  {
    TenantId? tenantId = query.TenantId;
    IUserSettings userSettings = query.UserSettings;

    Dictionary<UserId, User> users = new(capacity: 3);

    if (query.IncludeId && Guid.TryParse(query.User, out Guid id))
    {
      UserId userId = new(tenantId, new EntityId(id));
      User? user = await _userRepository.LoadAsync(userId, cancellationToken);
      if (user != null && user.TenantId == query.TenantId)
      {
        users[user.Id] = user;
      }
    }

    UniqueName? uniqueName = null;
    try
    {
      uniqueName = new(userSettings.UniqueName, query.User);
    }
    catch (Exception)
    {
    }
    if (uniqueName != null)
    {
      User? user = await _userRepository.LoadAsync(tenantId, uniqueName, cancellationToken);
      if (user != null)
      {
        users[user.Id] = user;
      }
    }

    if (userSettings.RequireUniqueEmail)
    {
      Email? email = null;
      try
      {
        email = new(query.User);
      }
      catch (Exception)
      {
      }
      if (email != null)
      {
        IEnumerable<User> foundUsers = await _userRepository.LoadAsync(tenantId, email, cancellationToken);
        if (foundUsers.Count() == 1)
        {
          User user = foundUsers.Single();
          users[user.Id] = user;
        }
      }
    }

    if (users.Count > 1)
    {
      throw TooManyResultsException<User>.ExpectedSingle(users.Count);
    }

    return users.Values.SingleOrDefault() ?? throw new UserNotFoundException(tenantId, query.User, query.PropertyName);
  }
}
