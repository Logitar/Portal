namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record ApiKeyRoleEntity
{
  public int ApiKeyId { get; set; }
  public int RoleId { get; set; }
}
