using Logitar.Identity.Core;
using Logitar.Identity.Core.Passwords;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Sessions.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchSessionsQueryTests : IntegrationTests
{
  private readonly IPasswordManager _passwordManager;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SearchSessionsQueryTests() : base()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return empty results when no session did match.")]
  public async Task It_should_return_empty_results_when_no_session_did_match()
  {
    SearchSessionsPayload payload = new()
    {
      IsPersistent = true
    };
    SearchSessionsQuery query = new(payload);
    SearchResults<SessionModel> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SetRealm();

    User user1 = new(new UniqueName(Realm.UniqueNameSettings, UsernameString), id: UserId.NewId(TenantId));
    User user2 = new(new UniqueName(Realm.UniqueNameSettings, Faker.Person.UserName), id: UserId.NewId(TenantId));
    await _userRepository.SaveAsync([user1, user2]);

    Session notInIds = user1.SignIn();
    Session otherUser = user2.SignIn();
    Session signedOut = user1.SignIn();
    signedOut.SignOut();
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out _);
    Session persistent = user1.SignIn(secret);
    Session session1 = user1.SignIn();
    Session session2 = user1.SignIn();
    await _sessionRepository.SaveAsync([notInIds, otherUser, signedOut, persistent, session1, session2]);

    SearchSessionsPayload payload = new()
    {
      UserId = user1.EntityId.ToGuid(),
      IsActive = true,
      IsPersistent = false,
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> sessionIds = (await _sessionRepository.LoadAsync()).Select(session => session.EntityId.ToGuid());
    payload.Ids.AddRange(sessionIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.EntityId.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("Hello World!"));
    payload.Sort.Add(new SessionSortOption(SessionSort.UpdatedOn, isDescending: true));
    SearchSessionsQuery query = new(payload);
    SearchResults<SessionModel> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    SessionModel session = Assert.Single(results.Items);
    Assert.Equal(session1.EntityId.ToGuid(), session.Id);
  }
}
