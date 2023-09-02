﻿namespace Logitar.Portal.EntityFrameworkCore.Relational.Entities;

internal record BlacklistedTokenEntity
{
  public BlacklistedTokenEntity(Guid id, DateTime? expiresOn = null)
  {
    Id = id;
    ExpiresOn = expiresOn;
  }

  private BlacklistedTokenEntity()
  {
  }

  public long BlacklistedTokenId { get; private set; }

  public Guid Id { get; private set; }

  public DateTime? ExpiresOn { get; private set; }
}
