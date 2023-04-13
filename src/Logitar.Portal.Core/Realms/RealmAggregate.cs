using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Portal.Core.Realms.Events;
using Logitar.Portal.Core.Realms.Validators;
using Logitar.Portal.Core.Senders;
using Logitar.Portal.Core.Templates;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Logitar.Portal.Core.Realms;

public class RealmAggregate : AggregateRoot
{
  private readonly Dictionary<string, ReadOnlyClaimMapping> _claimMappings = new();
  private readonly Dictionary<string, string> _customAttributes = new();

  public RealmAggregate(AggregateId id) : base(id)
  {
  }

  public RealmAggregate(AggregateId actorId, string uniqueName, string? displayName = null,
    string? description = null, CultureInfo? defaultLocale = null, string? secret = null, Uri? url = null,
    bool requireConfirmedAccount = false, bool requireUniqueEmail = false,
    ReadOnlyUsernameSettings? usernameSettings = null, ReadOnlyPasswordSettings? passwordSettings = null,
    Dictionary<string, ReadOnlyClaimMapping>? claimMappings = null,
    Dictionary<string, string>? customAttributes = null) : base()
  {
    RealmCreated e = new()
    {
      ActorId = actorId,
      UniqueName = uniqueName.Trim(),
      DisplayName = displayName?.CleanTrim(),
      Description = description?.CleanTrim(),
      DefaultLocale = defaultLocale,
      Secret = secret?.CleanTrim() ?? GenerateSecret(),
      Url = url,
      RequireConfirmedAccount = requireConfirmedAccount,
      RequireUniqueEmail = requireUniqueEmail,
      UsernameSettings = usernameSettings ?? new(),
      PasswordSettings = passwordSettings ?? new(),
      ClaimMappings = claimMappings?.CleanTrim() ?? new(),
      CustomAttributes = customAttributes?.CleanTrim() ?? new()
    };
    new RealmCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }

  public string UniqueName { get; private set; } = string.Empty;
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public CultureInfo? DefaultLocale { get; private set; }
  public string Secret { get; private set; } = string.Empty;
  public Uri? Url { get; private set; }

  public bool RequireConfirmedAccount { get; private set; }
  public bool RequireUniqueEmail { get; private set; }

  public ReadOnlyUsernameSettings UsernameSettings { get; private set; } = new();
  public ReadOnlyPasswordSettings PasswordSettings { get; private set; } = new();

  public IReadOnlyDictionary<string, ReadOnlyClaimMapping> ClaimMappings => _claimMappings.AsReadOnly();
  public IReadOnlyDictionary<string, string> CustomAttributes => _customAttributes.AsReadOnly();

  public AggregateId? PasswordRecoverySenderId { get; private set; }
  public AggregateId? PasswordRecoveryTemplateId { get; private set; }

  protected virtual void Apply(RealmCreated e)
  {
    UniqueName = e.UniqueName;

    Apply((RealmSaved)e);
  }

  public void Delete(AggregateId actorId) => ApplyChange(new RealmDeleted { ActorId = actorId });
  protected virtual void Apply(RealmDeleted _) { }

  public void SetPasswordRecoverySender(AggregateId actorId, SenderAggregate? sender)
  {
    if (PasswordRecoverySenderId != sender?.Id)
    {
      ApplyChange(new PasswordRecoverySenderChanged
      {
        ActorId = actorId,
        SenderId = sender?.Id
      });
    }
  }
  protected virtual void Apply(PasswordRecoverySenderChanged e) => PasswordRecoverySenderId = e.SenderId;

  public void SetPasswordRecoveryTemplate(AggregateId actorId, TemplateAggregate? template)
  {
    if (PasswordRecoveryTemplateId != template?.Id)
    {
      ApplyChange(new PasswordRecoveryTemplateChanged
      {
        ActorId = actorId,
        TemplateId = template?.Id
      });
    }
  }
  protected virtual void Apply(PasswordRecoveryTemplateChanged e) => PasswordRecoveryTemplateId = e.TemplateId;

  public void SetUrl(AggregateId actorId, Uri? url) => ApplyChange(new UrlChanged
  {
    ActorId = actorId,
    Url = url
  });
  protected virtual void Apply(UrlChanged e) => Url = e.Url;

  public void Update(AggregateId actorId, string? displayName, string? description,
    CultureInfo? defaultLocale, string? secret, Uri? url,
    bool requireConfirmedAccount, bool requireUniqueEmail,
    ReadOnlyUsernameSettings? usernameSettings, ReadOnlyPasswordSettings? passwordSettings,
    Dictionary<string, ReadOnlyClaimMapping>? claimMappings,
    Dictionary<string, string>? customAttributes)
  {
    RealmUpdated e = new()
    {
      ActorId = actorId,
      DisplayName = displayName?.CleanTrim(),
      Description = description?.CleanTrim(),
      DefaultLocale = defaultLocale,
      Secret = secret?.CleanTrim() ?? GenerateSecret(),
      Url = url,
      RequireConfirmedAccount = requireConfirmedAccount,
      RequireUniqueEmail = requireUniqueEmail,
      UsernameSettings = usernameSettings ?? new(),
      PasswordSettings = passwordSettings ?? new(),
      ClaimMappings = claimMappings?.CleanTrim() ?? new(),
      CustomAttributes = customAttributes?.CleanTrim() ?? new()
    };
    new RealmUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(RealmUpdated e) => Apply((RealmSaved)e);

  private void Apply(RealmSaved e)
  {
    DisplayName = e.DisplayName;
    Description = e.Description;

    DefaultLocale = e.DefaultLocale;
    Secret = e.Secret;
    Url = e.Url;

    RequireConfirmedAccount = e.RequireConfirmedAccount;
    RequireUniqueEmail = e.RequireUniqueEmail;

    UsernameSettings = e.UsernameSettings;
    PasswordSettings = e.PasswordSettings;

    _claimMappings.Clear();
    _claimMappings.AddRange(e.ClaimMappings);

    _customAttributes.Clear();
    _customAttributes.AddRange(e.CustomAttributes);
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";

  private static string GenerateSecret(int length = 256 / 8)
  {
    while (true)
    {
      /* In the ASCII table, there are 94 characters between 33 '!' and 126 '~' (126 - 33 + 1 = 94).
       * Since there are a total of 256 possibilities, by dividing per 94 and adding a 10% margin we
       * generate just a little more bytes than required, obtaining the factor 3. */
      byte[] bytes = RandomNumberGenerator.GetBytes(length * 3);

      List<byte> secret = new(capacity: length);
      for (int i = 0; i < bytes.Length; i++)
      {
        byte @byte = bytes[i];
        if (@byte >= 33 && @byte <= 126)
        {
          secret.Add(@byte);

          if (secret.Count == length)
          {
            return Encoding.ASCII.GetString(secret.ToArray());
          }
        }
      }
    }
  }
}
