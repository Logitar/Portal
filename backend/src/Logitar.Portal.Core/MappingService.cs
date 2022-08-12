using AutoMapper;
using Logitar.Portal.Core.Actors;
using Logitar.Portal.Core.Actors.Models;
using Logitar.Portal.Core.Sessions;
using Logitar.Portal.Core.Sessions.Models;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Models;

namespace Logitar.Portal.Core
{
  internal class MappingService : IMappingService
  {
    private readonly IActorService _actorService;
    private readonly IMapper _mapper;

    public MappingService(IActorService actorService, IMapper mapper)
    {
      _actorService = actorService;
      _mapper = mapper;
    }

    public async Task<T> MapAsync<T>(Aggregate aggregate, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(aggregate);

      var destination = _mapper.Map<T>(aggregate);

      if (destination is AggregateModel model)
      {
        Actor? createdBy = aggregate.CreatedById == Guid.Empty
          ? Actor.System
          : await _actorService.GetAsync(aggregate.CreatedById, cancellationToken);
        model.CreatedBy = _mapper.Map<ActorModel>(createdBy);

        if (aggregate.UpdatedById.HasValue)
        {
          Actor? updatedBy = aggregate.UpdatedById.Value == Guid.Empty
            ? Actor.System
            : await _actorService.GetAsync(aggregate.UpdatedById.Value, cancellationToken);
          model.UpdatedBy = _mapper.Map<ActorModel>(updatedBy);
        }
      }

      return destination;
    }

    public async Task<T> MapAsync<T>(Session session, CancellationToken cancellationToken)
    {
      T destination = await MapAsync<T>(aggregate: session, cancellationToken);

      if (destination is SessionModel model && session.SignedOutById.HasValue)
      {
        Actor? disabledBy = session.SignedOutById == Guid.Empty
          ? Actor.System
          : await _actorService.GetAsync(session.SignedOutById.Value, cancellationToken);
        model.SignedOutBy = _mapper.Map<ActorModel>(disabledBy);
      }

      return destination;
    }

    public async Task<T> MapAsync<T>(User user, CancellationToken cancellationToken)
    {
      T destination = await MapAsync<T>(aggregate: user, cancellationToken);

      if (destination is UserModel model && user.DisabledById.HasValue)
      {
        Actor? disabledBy = user.DisabledById == Guid.Empty
          ? Actor.System
          : await _actorService.GetAsync(user.DisabledById.Value, cancellationToken);
        model.DisabledBy = _mapper.Map<ActorModel>(disabledBy);
      }

      return destination;
    }

    public async Task<ListModel<TOut>> MapAsync<TIn, TOut>(PagedList<TIn> aggregates, CancellationToken cancellationToken)
      where TIn : Aggregate
      where TOut : AggregateModel
    {
      ArgumentNullException.ThrowIfNull(aggregates);

      HashSet<Guid> ids = aggregates.Select(x => x.CreatedById)
        .Union(aggregates.Where(x => x.UpdatedById.HasValue).Select(x => x.UpdatedById!.Value)).ToHashSet();
      Dictionary<Guid, ActorModel> actors = (await _actorService.GetAsync(ids, cancellationToken))
        .Select(x => _mapper.Map<ActorModel>(x))
        .ToDictionary(x => x.Id, x => x);
      actors.Add(Guid.Empty, _mapper.Map<ActorModel>(Actor.System));

      var models = new List<TOut>(capacity: aggregates.Count);
      foreach (TIn aggregate in aggregates)
      {
        var model = _mapper.Map<TOut>(aggregate);
        models.Add(model);

        if (actors.TryGetValue(aggregate.CreatedById, out ActorModel? createdBy))
        {
          model.CreatedBy = createdBy;
        }

        if (aggregate.UpdatedById.HasValue && actors.TryGetValue(aggregate.UpdatedById.Value, out ActorModel? updatedBy))
        {
          model.UpdatedBy = updatedBy;
        }
      }

      return new ListModel<TOut>(models, aggregates.Total);
    }

    public async Task<ListModel<T>> MapAsync<T>(PagedList<Session> sessions, CancellationToken cancellationToken)
      where T : AggregateModel
    {
      ArgumentNullException.ThrowIfNull(sessions);

      HashSet<Guid> ids = sessions.Select(x => x.CreatedById)
        .Union(sessions.Where(x => x.SignedOutById.HasValue).Select(x => x.SignedOutById!.Value))
        .Union(sessions.Where(x => x.UpdatedById.HasValue).Select(x => x.UpdatedById!.Value)).ToHashSet();
      Dictionary<Guid, ActorModel> actors = (await _actorService.GetAsync(ids, cancellationToken))
        .Select(x => _mapper.Map<ActorModel>(x))
        .ToDictionary(x => x.Id, x => x);
      actors.Add(Guid.Empty, _mapper.Map<ActorModel>(Actor.System));

      var models = new List<T>(capacity: sessions.Count);
      foreach (Session session in sessions)
      {
        var model = _mapper.Map<T>(session);
        models.Add(model);

        if (actors.TryGetValue(session.CreatedById, out ActorModel? createdBy))
        {
          model.CreatedBy = createdBy;
        }

        if (session.UpdatedById.HasValue && actors.TryGetValue(session.UpdatedById.Value, out ActorModel? updatedBy))
        {
          model.UpdatedBy = updatedBy;
        }

        if (model is SessionModel sessionModel
          && session.SignedOutById.HasValue
          && actors.TryGetValue(session.SignedOutById.Value, out ActorModel? signedOutBy))
        {
          sessionModel.SignedOutBy = signedOutBy;
        }
      }

      return new ListModel<T>(models, sessions.Total);
    }
  }
}
