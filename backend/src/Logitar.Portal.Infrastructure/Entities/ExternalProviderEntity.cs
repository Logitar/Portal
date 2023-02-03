﻿using Logitar.Portal.Domain.Users.Events;

namespace Logitar.Portal.Infrastructure.Entities
{
  internal class ExternalProviderEntity
  {
    public ExternalProviderEntity(UserAddedExternalProviderEvent @event, UserEntity user)
    {
      if (user.Realm == null)
      {
        throw new ArgumentException($"The {nameof(user.Realm)} is required.", nameof(user));
      }

      Id = Guid.NewGuid();

      User = user;
      UserId = user.UserId;

      Realm = user.Realm;
      RealmId = user.Realm.RealmId;

      Key = @event.Key;
      Value = @event.Value;
      DisplayName = @event.DisplayName;

      AddedBy = @event.UserId.Value;
      AddedOn = @event.OccurredOn;
    }
    private ExternalProviderEntity()
    {
    }

    public int ExternalProviderId { get; private set; }

    public Guid Id { get; private set; }

    public RealmEntity? Realm { get; private set; }
    public int RealmId { get; private set; }

    public UserEntity? User { get; private set; }
    public int UserId { get; private set; }

    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }

    public string AddedBy { get; private set; } = string.Empty;
    public DateTime AddedOn { get; private set; }
  }
}