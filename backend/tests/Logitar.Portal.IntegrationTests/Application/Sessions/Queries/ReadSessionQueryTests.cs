using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Contracts.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Application.Sessions.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadSessionQueryTests : IntegrationTests
{
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
    SessionEntity? entity = await IdentityContext.Sessions.AsNoTracking().SingleOrDefaultAsync();
    Assert.NotNull(entity);
    Guid id = new AggregateId(entity.AggregateId).ToGuid();

    ReadSessionQuery query = new(id);
    Session? session = await Mediator.Send(query);
    Assert.NotNull(session);
    Assert.Equal(id, session.Id);
  }
}
