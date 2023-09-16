using Logitar.Portal.Contracts.Messages;

namespace Logitar.Portal.Domain.Messages;

public class MissingToRecipientException : Exception
{
  public MissingToRecipientException() : base($"At least one {RecipientType.To} recipient is required.")
  {
  }
}
