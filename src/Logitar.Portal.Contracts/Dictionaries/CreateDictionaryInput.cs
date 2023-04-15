﻿namespace Logitar.Portal.Contracts.Dictionaries;

public record CreateDictionaryInput
{
  public string? Realm { get; set; }
  public string Locale { get; set; } = string.Empty;

  public IEnumerable<Entry>? Entries { get; set; }
}
