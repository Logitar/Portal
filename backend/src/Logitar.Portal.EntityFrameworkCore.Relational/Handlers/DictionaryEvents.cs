using Logitar.Portal.Domain.Dictionaries.Events;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Handlers;

internal class DictionaryEvents : INotificationHandler<DictionaryCreated>,
  INotificationHandler<DictionaryDeleted>,
  INotificationHandler<DictionaryLocaleChanged>,
  INotificationHandler<DictionaryUpdated>

{
  private readonly PortalContext _context;

  public DictionaryEvents(PortalContext context)
  {
    _context = context;
  }

  public async Task Handle(DictionaryCreated @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await _context.Dictionaries.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (dictionary == null)
    {
      dictionary = new(@event);

      _context.Dictionaries.Add(dictionary);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(DictionaryDeleted @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await _context.Dictionaries
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (dictionary != null)
    {
      _context.Dictionaries.Remove(dictionary);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(DictionaryLocaleChanged @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await _context.Dictionaries
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (dictionary != null)
    {
      dictionary.SetLocale(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(DictionaryUpdated @event, CancellationToken cancellationToken)
  {
    DictionaryEntity? dictionary = await _context.Dictionaries
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (dictionary != null)
    {
      dictionary.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
