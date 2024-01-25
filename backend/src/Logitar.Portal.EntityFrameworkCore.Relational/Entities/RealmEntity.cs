using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Portal.Domain.Realms.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal class RealmEntity : AggregateEntity
{
  public int RealmId { get; private set; }

  public string UniqueSlug { get; private set; } = string.Empty;
  public string UniqueSlugNormalized
  {
    get => UniqueSlug.ToUpper();
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string? DefaultLocale { get; private set; }
  public string Secret { get; private set; } = string.Empty;
  public string? Url { get; private set; }

  public string? AllowedUniqueNameCharacters { get; private set; }
  public int PasswordsRequiredLength { get; private set; }
  public int PasswordsRequiredUniqueChars { get; private set; }
  public bool PasswordsRequireNonAlphanumeric { get; private set; }
  public bool PasswordsRequireLowercase { get; private set; }
  public bool PasswordsRequireUppercase { get; private set; }
  public bool PasswordsRequireDigit { get; private set; }
  public string PasswordsHashingStrategy { get; private set; } = string.Empty;
  public bool RequireUniqueEmail { get; private set; }

  public Dictionary<string, string> CustomAttributes { get; private set; } = [];
  public string? CustomAttributesSerialized
  {
    get => CustomAttributes.Count == 0 ? null : JsonSerializer.Serialize(CustomAttributes);
    private set
    {
      if (value == null)
      {
        CustomAttributes.Clear();
      }
      else
      {
        CustomAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? [];
      }
    }
  }

  public RealmEntity(RealmCreatedEvent @event) : base(@event)
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

  public void SetUniqueSlug(RealmUniqueSlugChangedEvent @event)
  {
    Update(@event);

    UniqueSlug = @event.UniqueSlug.Value;
  }

  public void Update(RealmUpdatedEvent @event)
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

    foreach (KeyValuePair<string, string?> customAttribute in @event.CustomAttributes)
    {
      if (customAttribute.Value == null)
      {
        CustomAttributes.Remove(customAttribute.Key);
      }
      else
      {
        CustomAttributes[customAttribute.Key] = customAttribute.Value;
      }
    }
  }

  private void SetPasswordSettings(IPasswordSettings password)
  {
    PasswordsRequiredLength = password.RequiredLength;
    PasswordsRequiredUniqueChars = password.RequiredUniqueChars;
    PasswordsRequireNonAlphanumeric = password.RequireNonAlphanumeric;
    PasswordsRequireLowercase = password.RequireLowercase;
    PasswordsRequireUppercase = password.RequireUppercase;
    PasswordsRequireDigit = password.RequireDigit;
    PasswordsHashingStrategy = password.HashingStrategy;
  }
  private void SetUniqueNameSettings(IUniqueNameSettings uniqueName)
  {
    AllowedUniqueNameCharacters = uniqueName.AllowedCharacters;
  }
}
