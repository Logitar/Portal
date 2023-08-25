namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record UserRoleEntity
{
  public int UserId { get; set; }
  public int RoleId { get; set; }
}
