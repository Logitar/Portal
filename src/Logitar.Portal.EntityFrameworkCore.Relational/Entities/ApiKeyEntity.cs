using Logitar.Portal.Contracts;
using Logitar.Portal.Domain.ApiKeys.Events;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record ApiKeyEntity : AggregateEntity
{
  public ApiKeyEntity(ApiKeyCreatedEvent created) : base(created)
  {
    TenantId = created.TenantId;

    Secret = created.Secret?.Encode();

    Title = created.DisplayName;
  }

  private ApiKeyEntity() : base()
  {
  }

  public int ApiKeyId { get; private set; }

  public string? TenantId { get; private set; }

  public string? Secret { get; private set; }

  public string Title { get; private set; } = string.Empty;
  public string? Description { get; private set; }
  public DateTime? ExpiresOn { get; private set; }

  public DateTime? AuthenticatedOn { get; set; }

  public Dictionary<string, string> CustomAttributes { get; private set; } = new();
  public string? CustomAttributesSerialized
  {
    get => CustomAttributes.Any() ? JsonSerializer.Serialize(CustomAttributes) : null;
    private set
    {
      if (value == null)
      {
        CustomAttributes.Clear();
      }
      else
      {
        CustomAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(value) ?? new();
      }
    }
  }

  public List<RoleEntity> Roles { get; } = new();

  public void Authenticate(ApiKeyAuthenticatedEvent authenticated)
  {
    Update(authenticated);

    AuthenticatedOn = authenticated.OccurredOn.ToUniversalTime();
  }

  public void Update(ApiKeyUpdatedEvent updated, IEnumerable<RoleEntity> roles)
  {
    Update(updated);

    if (updated.DisplayName != null)
    {
      Title = updated.DisplayName;
    }
    if (updated.Description != null)
    {
      Description = updated.Description.Value;
    }
    if (updated.ExpiresOn.HasValue)
    {
      ExpiresOn = updated.ExpiresOn.Value.ToUniversalTime();
    }

    foreach (KeyValuePair<string, string?> customAttribute in updated.CustomAttributes)
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

    Dictionary<string, RoleEntity> roleById = roles.ToDictionary(x => x.AggregateId, x => x);
    foreach (KeyValuePair<string, CollectionAction> roleAction in updated.Roles)
    {
      if (roleById.TryGetValue(roleAction.Key, out RoleEntity? role))
      {
        switch (roleAction.Value)
        {
          case CollectionAction.Add:
            Roles.Add(role);
            break;
          case CollectionAction.Remove:
            Roles.Remove(role);
            break;
        }
      }
    }
  }
}
