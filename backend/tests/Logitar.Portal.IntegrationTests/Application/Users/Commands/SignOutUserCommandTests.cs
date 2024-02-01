using Logitar.EventSourcing;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Users.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignOutUserCommandTests : IntegrationTests
{
  private readonly IUserRepository _userRepository;

  public SignOutUserCommandTests() : base()
  {
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return null when the user cannot be found.")]
  public async Task It_should_return_null_when_the_user_cannot_be_found()
  {
    SignOutUserCommand command = new(Guid.NewGuid());
    User? user = await Mediator.Send(command);
    Assert.Null(user);
  }

  [Fact(DisplayName = "It should return null when the user is not in the realm.")]
  public async Task It_should_return_null_when_the_user_is_not_in_the_realm()
  {
    SetRealm();

    SignOutUserCommand command = new(Guid.NewGuid());
    User? user = await Mediator.Send(command);
    Assert.Null(user);
  }

  [Fact(DisplayName = "It should sign-out the specified user.")]
  public async Task It_should_sign_out_the_specified_user()
  {
    UserAggregate aggregate = (await _userRepository.LoadAsync()).Single();
    SignOutUserCommand command = new(aggregate.Id.AggregateId.ToGuid());

    User? user = await Mediator.Send(command);
    Assert.NotNull(user);
    Assert.Equal(command.Id, user.Id);

    string aggregateId = new AggregateId(command.Id).Value;
    SessionEntity[] sessions = await IdentityContext.Sessions.AsNoTracking()
      .Include(x => x.User)
      .Where(x => x.User!.AggregateId == aggregateId)
      .ToArrayAsync();
    foreach (SessionEntity session in sessions)
    {
      Assert.False(session.IsActive);
      Assert.NotNull(session.SignedOutBy);
      Assert.NotNull(session.SignedOutOn);
    }
  }
}
