using Logitar.Portal.v2.Contracts.Messages;

namespace Logitar.Portal.v2.Core.Messages;

internal record Recipients(IEnumerable<Recipient> To, IEnumerable<Recipient> CC, IEnumerable<Recipient> Bcc);
