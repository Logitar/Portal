using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Contracts.Tokens;

public record ValidatedTokenModel
{
  public string? Subject { get; set; }
  public EmailModel? Email { get; set; }
  public List<ClaimModel> Claims { get; set; } = [];
}
