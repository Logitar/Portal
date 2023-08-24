namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record RoleEntity : AggregateEntity
{
  private RoleEntity() : base()
  {
  }

  public int RoleId { get; private set; }

  public string? TenantId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => UniqueName.ToUpper();
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

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

  public List<UserEntity> Users { get; } = new();
}
