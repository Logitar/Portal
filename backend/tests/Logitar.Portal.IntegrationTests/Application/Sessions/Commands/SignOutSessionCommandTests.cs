using Logitar.Identity.Domain.Sessions;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Domain.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Sessions.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignOutSessionCommandTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;

  public SignOutSessionCommandTests() : base()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
  }

  [Fact(DisplayName = "It should return null when the session cannot be found.")]
  public async Task It_should_return_null_when_the_session_cannot_be_found()
  {
    SignOutSessionCommand command = new(Guid.NewGuid());
    Session? session = await Mediator.Send(command);
    Assert.Null(session);
  }

  [Fact(DisplayName = "It should return null when the user is not in the realm.")]
  public async Task It_should_return_null_when_the_user_is_not_in_the_realm()
  {
    Realm realm = new("tests", JwtSecretUnit.Generate().Value)
    {
      Id = Guid.NewGuid()
    };
    SetRealm(realm);

    SignOutSessionCommand command = new(Guid.NewGuid());
    Session? session = await Mediator.Send(command);
    Assert.Null(session);
  }

  [Fact(DisplayName = "It should sign-out the specified session.")]
  public async Task It_should_sign_out_the_specified_session()
  {
    SessionAggregate aggregate = (await _sessionRepository.LoadAsync()).Single();
    SignOutSessionCommand command = new(aggregate.Id.AggregateId.ToGuid());

    Session? session = await Mediator.Send(command);
    Assert.NotNull(session);
    Assert.False(session.IsActive);
    Assert.Equal(aggregate.Id.AggregateId.ToGuid(), session.Id);
    Assert.NotNull(session.SignedOutBy);
    Assert.NotNull(session.SignedOutOn);
  }
}
