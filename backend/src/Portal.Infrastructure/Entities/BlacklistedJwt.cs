﻿namespace Portal.Infrastructure.Entities
{
  internal class BlacklistedJwt
  {
    public BlacklistedJwt(Guid id, DateTime? expiresAt = null)
    {
      Id = id;
      ExpiresAt = expiresAt;
    }
    private BlacklistedJwt()
    {
    }

    public Guid Id { get; private set; }
    public long Sid { get; private set; }

    public DateTime? ExpiresAt { get; private set; }
  }
}
