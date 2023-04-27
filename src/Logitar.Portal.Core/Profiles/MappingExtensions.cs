using AutoMapper;
using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;

namespace Logitar.Portal.Core.Profiles;

internal static class MappingExtensions
{
  internal const string ActorsKey = "Actors";

  public static Actor GetActor(this ResolutionContext context, AggregateId id)
  {
    if (!context.Items.TryGetValue(ActorsKey, out object? value))
    {
      throw new ArgumentException($"The '{ActorsKey}' item has not been set.", nameof(context));
    }
    else if (value is not IReadOnlyDictionary<AggregateId, Actor> actors)
    {
      throw new ArgumentException($"The '{ActorsKey}' item should implement the '{nameof(IReadOnlyDictionary<AggregateId, Actor>)}' interface.", nameof(context));
    }
    else
    {
      return actors[id];
    }
  }
}
