using Logitar.EventSourcing;
using Logitar.Identity.Core.Sessions;
using Logitar.Identity.Core.Users;
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
    SessionModel? session = await ActivityPipeline.ExecuteAsync(query);
    Assert.Null(session);
  }

  [Fact(DisplayName = "It should return the session when it is found.")]
  public async Task It_should_return_the_session_when_it_is_found()
  {
    User user = Assert.Single(await _userRepository.LoadAsync());
    Session aggregate = new(user);
    await _sessionRepository.SaveAsync(aggregate);

    SessionEntity? entity = await IdentityContext.Sessions.AsNoTracking().SingleOrDefaultAsync();
    Assert.NotNull(entity);
    Guid id = new SessionId(new StreamId(entity.StreamId)).EntityId.ToGuid();

    ReadSessionQuery query = new(id);
    SessionModel? session = await ActivityPipeline.ExecuteAsync(query);
    Assert.NotNull(session);
    Assert.Equal(id, session.Id);
  }
}
