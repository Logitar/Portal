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
    SearchResults<UserModel> results = await ActivityPipeline.ExecuteAsync(query);
    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    SetRealm();

    User initialUser = Assert.Single(await _userRepository.LoadAsync());
    User noNickname = new(new UniqueName(Realm.UniqueNameSettings, "NoNickname"), TenantId);
    User notInIds = new(new UniqueName(Realm.UniqueNameSettings, "NotInIds"), TenantId);
    User noPassword = new(new UniqueName(Realm.UniqueNameSettings, "NoPassword"), TenantId);
    User disabled = new(new UniqueName(Realm.UniqueNameSettings, "Disabled"), TenantId);
    User confirmed = new(new UniqueName(Realm.UniqueNameSettings, "Confirmed"), TenantId);

    disabled.Disable();
    confirmed.SetEmail(new Email(Faker.Internet.Email(), isVerified: true));

    User youngest = new(new UniqueName(Realm.UniqueNameSettings, Faker.Internet.UserName()), TenantId)
    {
      Birthdate = DateTime.Now.AddYears(-20)
    };
    User oldest = new(new UniqueName(Realm.UniqueNameSettings, Faker.Internet.UserName()), TenantId)
    {
      Birthdate = DateTime.Now.AddYears(-30)
    };

    PersonName nickname = new(Faker.Name.FirstName());
    User[] users = [initialUser, notInIds, noPassword, disabled, confirmed, youngest, oldest];
    foreach (User user in users)
    {
      user.Nickname = nickname;
      user.Update();
    }

    Password password = _passwordManager.ValidateAndCreate(PasswordString);
    users = [noNickname, notInIds, disabled, confirmed, youngest, oldest];
    foreach (User user in users)
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
    SearchResults<UserModel> results = await ActivityPipeline.ExecuteAsync(query);

    Assert.Equal(2, results.Total);
    UserModel result = Assert.Single(results.Items);
    Assert.Equal(youngest.Id.ToGuid(), result.Id);
  }
}
