﻿using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Sessions;

public record SessionSortOption : SortOption
{
  public new SessionSort Field
  {
    get => Enum.Parse<SessionSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public SessionSortOption() : this(SessionSort.UpdatedOn, isDescending: true)
  {
  }

  public SessionSortOption(SessionSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
