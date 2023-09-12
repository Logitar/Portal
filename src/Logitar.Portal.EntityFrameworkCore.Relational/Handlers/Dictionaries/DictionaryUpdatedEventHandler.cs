using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers.Dictionaries;

internal class DictionaryUpdatedEventHandler : INotificationHandler<DictionaryUpdatedEvent>
{
  private readonly PortalContext _context;

  public DictionaryUpdatedEventHandler(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(DictionaryUpdatedEvent @event, CancellationToken cancellationToken)
  {
    DictionaryEntity dictionary = await _context.Dictionaries
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new EntityNotFoundException<DictionaryEntity>(@event.AggregateId);

    dictionary.Update(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
