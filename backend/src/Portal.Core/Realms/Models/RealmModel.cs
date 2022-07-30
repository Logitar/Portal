﻿namespace Portal.Core.Realms.Models
{
  public class RealmModel : AggregateModel
  {
    public string Alias { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool RequireConfirmedAccount { get; set; }
    public string? Url { get; set; }

    public Guid? PasswordRecoverySenderId { get; set; }
    public Guid? PasswordRecoveryTemplateId { get; set; }
  }
}
