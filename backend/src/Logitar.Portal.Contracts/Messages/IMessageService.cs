﻿using Logitar.Portal.Contracts.Search;

namespace Logitar.Portal.Contracts.Messages;

public interface IMessageService
{
  Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<SearchResults<Message>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken = default);
  Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken = default);
}
