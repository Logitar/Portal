using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
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
    SearchResults<Session> results = await Mediator.Send(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SetRealm();

    UserAggregate user1 = new(new UniqueNameUnit(Realm.UniqueNameSettings, Faker.Person.UserName), TenantId);
    UserAggregate user2 = new(new UniqueNameUnit(Realm.UniqueNameSettings, Faker.Internet.UserName()), TenantId);
    await _userRepository.SaveAsync([user1, user2]);

    SessionAggregate notInIds = user1.SignIn();
    SessionAggregate otherUser = user2.SignIn();
    SessionAggregate signedOut = user1.SignIn();
    signedOut.SignOut();
    Password secret = _passwordManager.GenerateBase64(RefreshToken.SecretLength, out _);
    SessionAggregate persistent = user1.SignIn(secret);
    SessionAggregate session1 = user1.SignIn();
    SessionAggregate session2 = user1.SignIn();
    await _sessionRepository.SaveAsync([notInIds, otherUser, signedOut, persistent, session1, session2]);

    SearchSessionsPayload payload = new()
    {
      UserId = user1.Id.ToGuid(),
      IsActive = true,
      IsPersistent = false,
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> sessionIds = (await _sessionRepository.LoadAsync()).Select(session => session.Id.ToGuid());
    payload.Ids.AddRange(sessionIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.Id.ToGuid());
    payload.Search.Terms.Add(new SearchTerm("Hello World!"));
    payload.Sort.Add(new SessionSortOption(SessionSort.UpdatedOn, isDescending: true));
    SearchSessionsQuery query = new(payload);
    SearchResults<Session> results = await Mediator.Send(query);

    Assert.Equal(2, results.Total);
    Session session = Assert.Single(results.Items);
    Assert.Equal(session1.Id.ToGuid(), session.Id);
  }
}
