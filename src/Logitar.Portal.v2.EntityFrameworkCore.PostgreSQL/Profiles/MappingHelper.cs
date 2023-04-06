﻿using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Actors;
using Logitar.Portal.v2.Contracts.Realms;
using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using System.Text.Json;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Profiles;

internal static class MappingHelper
{
  public static Actor GetActor(Guid id, string json)
  {
    ActorEntity entity = ActorEntity.Deserialize(json);

    return new Actor
    {
      Id = id,
      Type = entity.Type,
      IsDeleted = entity.IsDeleted,
      DisplayName = entity.DisplayName,
      EmailAddress = entity.EmailAddress,
      Picture = entity.Picture
    };
  }

  public static IEnumerable<CustomAttribute> GetCustomAttributes(ICustomAttributes entity, object? _)
  {
    if (entity.CustomAttributes == null)
    {
      return Enumerable.Empty<CustomAttribute>();
    }

    Dictionary<string, string> customAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(entity.CustomAttributes)
      ?? throw new InvalidOperationException($"The custom attributes deserialization failed: '{entity.CustomAttributes}'.");

    return customAttributes.Select(pair => new CustomAttribute
    {
      Key = pair.Key,
      Value = pair.Value
    });
  }

  public static Guid GetId(AggregateEntity entity, object? _)
  {
    return new AggregateId(entity.AggregateId).ToGuid();
  }
}
