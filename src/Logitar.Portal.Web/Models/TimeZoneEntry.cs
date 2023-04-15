﻿namespace Logitar.Portal.Web.Models;

public record TimeZoneEntry
{
  public TimeZoneEntry(string id)
  {
    Id = id;
    DisplayName = id.Replace('_', ' ');
  }

  public string Id { get; }
  public string DisplayName { get; }
}
