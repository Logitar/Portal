using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Core.Messages;

internal record Recipients(IEnumerable<Recipient> To, IEnumerable<Recipient> CC, IEnumerable<Recipient> Bcc);
