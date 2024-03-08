using Logitar.Identity.Domain.Passwords;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchUsersQueryTests : IntegrationTests
{
  private readonly IPasswordManager _passwordManager;
  private readonly IUserRepository _userRepository;

  public SearchUsersQueryTests() : base()
  {
    _passwordManager = ServiceProvider.GetRequiredService<IPasswordManager>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return empty results when no user did match.")]
  public async Task It_should_return_empty_results_when_no_user_did_match()
  {
    SearchUsersPayload payload = new()
    {
      IsConfirmed = true
    };
    SearchUsersQuery query = new(payload);
    SearchResults<User> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SetRealm();

    UserAggregate initialUser = Assert.Single(await _userRepository.LoadAsync());
    UserAggregate noNickname = new(new UniqueNameUnit(Realm.UniqueNameSettings, "NoNickname"), TenantId);
    UserAggregate notInIds = new(new UniqueNameUnit(Realm.UniqueNameSettings, "NotInIds"), TenantId);
    UserAggregate noPassword = new(new UniqueNameUnit(Realm.UniqueNameSettings, "NoPassword"), TenantId);
    UserAggregate disabled = new(new UniqueNameUnit(Realm.UniqueNameSettings, "Disabled"), TenantId);
    UserAggregate confirmed = new(new UniqueNameUnit(Realm.UniqueNameSettings, "Confirmed"), TenantId);

    disabled.Disable();
    confirmed.SetEmail(new EmailUnit(Faker.Internet.Email(), isVerified: true));

    UserAggregate youngest = new(new UniqueNameUnit(Realm.UniqueNameSettings, Faker.Internet.UserName()), TenantId)
    {
      Birthdate = DateTime.Now.AddYears(-20)
    };
    UserAggregate oldest = new(new UniqueNameUnit(Realm.UniqueNameSettings, Faker.Internet.UserName()), TenantId)
    {
      Birthdate = DateTime.Now.AddYears(-30)
    };

    PersonNameUnit nickname = new(Faker.Name.FirstName());
    UserAggregate[] users = [initialUser, notInIds, noPassword, disabled, confirmed, youngest, oldest];
    foreach (UserAggregate user in users)
    {
      user.Nickname = nickname;
      user.Update();
    }

    Password password = _passwordManager.ValidateAndCreate(PasswordString);
    users = [noNickname, notInIds, disabled, confirmed, youngest, oldest];
    foreach (UserAggregate user in users)
    {
      user.SetPassword(password);
    }

    await _userRepository.SaveAsync([initialUser, noNickname, notInIds, noPassword, disabled, confirmed, youngest, oldest]);

    SearchUsersPayload payload = new()
    {
      HasPassword = true,
      IsDisabled = false,
      IsConfirmed = false,
      Skip = 1,
      Limit = 1
    };
    IEnumerable<Guid> userIds = (await _userRepository.LoadAsync()).Select(user => user.Id.ToGuid());
    payload.Ids.AddRange(userIds);
    payload.Ids.Add(Guid.NewGuid());
    payload.Ids.Remove(notInIds.Id.ToGuid());
    payload.Search.Terms.Add(new SearchTerm(nickname.Value));
    payload.Sort.Add(new UserSortOption(UserSort.Birthdate, isDescending: false));
    SearchUsersQuery query = new(payload);
    SearchResults<User> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    User result = Assert.Single(results.Items);
    Assert.Equal(youngest.Id.ToGuid(), result.Id);
  }
}
