﻿using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Realms.Models;
using Logitar.Portal.Core.Realms.Payloads;

namespace Logitar.Portal.Core.Realms
{
  public interface IRealmService
  {
    Task<RealmModel> CreateAsync(CreateRealmPayload payload, CancellationToken cancellationToken = default);
    Task<RealmModel> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<RealmModel?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<ListModel<RealmModel>> GetAsync(string? search = null,
      RealmSort? sort = null, bool isDescending = false,
      int? index = null, int? count = null,
      CancellationToken cancellationToken = default);
    Task<RealmModel> UpdateAsync(string id, UpdateRealmPayload payload, CancellationToken cancellationToken = default);
  }
}
