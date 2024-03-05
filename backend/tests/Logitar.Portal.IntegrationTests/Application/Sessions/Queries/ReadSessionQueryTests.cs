using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.Application.Sessions.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadSessionQueryTests : IntegrationTests
{
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public ReadSessionQueryTests()
  {
    _sessionRepository = ServiceProvider.GetRequiredService<ISessionRepository>();
    _userRepository = ServiceProvider.GetRequiredService<IUserRepository>();
  }

  [Fact(DisplayName = "It should return null when the session cannot be found.")]
  public async Task It_should_return_null_when_the_session_cannot_be_found()
  {
    ReadSessionQuery query = new(Guid.NewGuid());
    Session? session = await Mediator.Send(query);
    Assert.Null(session);
  }

  [Fact(DisplayName = "It should return the session when it is found.")]
  public async Task It_should_return_the_session_when_it_is_found()
  {
    UserAggregate user = Assert.Single(await _userRepository.LoadAsync());
    SessionAggregate aggregate = new(user);
    await _sessionRepository.SaveAsync(aggregate);

    SessionEntity? entity = await IdentityContext.Sessions.AsNoTracking().SingleOrDefaultAsync();
    Assert.NotNull(entity);
    Guid id = new AggregateId(entity.AggregateId).ToGuid();

    ReadSessionQuery query = new(id);
    Session? session = await Mediator.Send(query);
    Assert.NotNull(session);
    Assert.Equal(id, session.Id);
  }
}
