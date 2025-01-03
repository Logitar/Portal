﻿using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;
using Logitar.Portal.Domain.Realms.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class RealmEntity : AggregateEntity
{
  public int RealmId { get; private set; }

  public string UniqueSlug { get; private set; } = string.Empty;
  public string UniqueSlugNormalized
  {
    get => Helper.Normalize(UniqueSlug);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string? DefaultLocale { get; private set; }
  public string Secret { get; private set; } = string.Empty;
  public string? Url { get; private set; }

  public string? AllowedUniqueNameCharacters { get; private set; }
  public int RequiredPasswordLength { get; private set; }
  public int RequiredPasswordUniqueChars { get; private set; }
  public bool PasswordsRequireNonAlphanumeric { get; private set; }
  public bool PasswordsRequireLowercase { get; private set; }
  public bool PasswordsRequireUppercase { get; private set; }
  public bool PasswordsRequireDigit { get; private set; }
  public string PasswordHashingStrategy { get; private set; } = string.Empty;
  public bool RequireUniqueEmail { get; private set; }

  public string? CustomAttributes { get; private set; }

  public RealmEntity(RealmCreated @event) : base(@event)
  {
    UniqueSlug = @event.UniqueSlug.Value;

    Secret = @event.Secret.Value;

    SetUniqueNameSettings(@event.UniqueNameSettings);
    SetPasswordSettings(@event.PasswordSettings);
    RequireUniqueEmail = @event.RequireUniqueEmail;
  }

  private RealmEntity() : base()
  {
  }

  public void SetUniqueSlug(RealmUniqueSlugChanged @event)
  {
    Update(@event);

    UniqueSlug = @event.UniqueSlug.Value;
  }

  public void Update(RealmUpdated @event)
  {
    base.Update(@event);

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }

    if (@event.DefaultLocale != null)
    {
      DefaultLocale = @event.DefaultLocale.Value?.Code;
    }
    if (@event.Secret != null)
    {
      Secret = @event.Secret.Value;
    }
    if (@event.Url != null)
    {
      Url = @event.Url.Value?.Value;
    }

    if (@event.UniqueNameSettings != null)
    {
      SetUniqueNameSettings(@event.UniqueNameSettings);
    }
    if (@event.PasswordSettings != null)
    {
      SetPasswordSettings(@event.PasswordSettings);
    }
    if (@event.RequireUniqueEmail.HasValue)
    {
      RequireUniqueEmail = @event.RequireUniqueEmail.Value;
    }

    Dictionary<string, string> customAttributes = GetCustomAttributes();
    foreach (KeyValuePair<Identifier, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        customAttributes.Remove(customAttribute.Key.Value);
      }
      else
      {
        customAttributes[customAttribute.Key.Value] = customAttribute.Value;
      }
    }
    SetCustomAttributes(customAttributes);
  }

  private void SetUniqueNameSettings(IUniqueNameSettings uniqueName)
  {
    AllowedUniqueNameCharacters = uniqueName.AllowedCharacters;
  }
  private void SetPasswordSettings(IPasswordSettings password)
  {
    RequiredPasswordLength = password.RequiredLength;
    RequiredPasswordUniqueChars = password.RequiredUniqueChars;
    PasswordsRequireNonAlphanumeric = password.RequireNonAlphanumeric;
    PasswordsRequireLowercase = password.RequireLowercase;
    PasswordsRequireUppercase = password.RequireUppercase;
    PasswordsRequireDigit = password.RequireDigit;
    PasswordHashingStrategy = password.HashingStrategy;
  }

  public Dictionary<string, string> GetCustomAttributes()
  {
    return (CustomAttributes == null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(CustomAttributes)) ?? [];
  }
  private void SetCustomAttributes(Dictionary<string, string> customAttributes)
  {
    CustomAttributes = customAttributes.Count < 1 ? null : JsonSerializer.Serialize(customAttributes);
  }
}
