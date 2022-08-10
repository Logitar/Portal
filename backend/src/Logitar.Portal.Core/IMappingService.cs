﻿using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core
{
  public interface IMappingService
  {
    Task<T> MapAsync<T>(Aggregate aggregate, CancellationToken cancellationToken = default);
    Task<T> MapAsync<T>(User user, CancellationToken cancellationToken = default);
    Task<ListModel<TOut>> MapAsync<TIn, TOut>(PagedList<TIn> aggregates, CancellationToken cancellationToken)
      where TIn : Aggregate
      where TOut : AggregateModel;
  }
}
