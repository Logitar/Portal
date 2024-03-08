using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignOutSessionCommandTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public SignOutSessionCommandTests() : base()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return null when the session cannot be found.")]
  public async Task It_should_return_null_when_the_session_cannot_be_found()
  {
    SignOutSessionCommand command = new(Guid.NewGuid());
    Session? session = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(session);
  }

  [Fact(DisplayName = "It should return null when the user is not in the realm.")]
  public async Task It_should_return_null_when_the_user_is_not_in_the_realm()
  {
    SetRealm();

    SignOutSessionCommand command = new(Guid.NewGuid());
    Session? session = await ActivityPipeline.ExecuteAsync(command);
    Assert.Null(session);
  }

  [Fact(DisplayName = "It should sign-out the specified session.")]
  public async Task It_should_sign_out_the_specified_session()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    SessionAggregate aggregate = new(user);
    await _sessionRepository.SaveAsync(aggregate);

    SignOutSessionCommand command = new(aggregate.Id.ToGuid());

    Session? session = await ActivityPipeline.ExecuteAsync(command);
    Assert.NotNull(session);
    Assert.False(session.IsActive);
    Assert.Equal(aggregate.Id.ToGuid(), session.Id);
    Assert.NotNull(session.SignedOutBy);
    Assert.NotNull(session.SignedOutOn);
  }
}
