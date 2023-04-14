﻿using Logitar.EventSourcing;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Dictionaries.Events;

public record DictionaryCreated : DictionarySaved, INotification
{
  public AggregateId? RealmId { get; init; }
  public CultureInfo Locale { get; init; } = null!;
}
