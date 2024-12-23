using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms;

public class UniqueSlugAlreadyUsedException : Exception
{
  public const string ErrorMessage = "The specified unique slug is already used.";

  public string UniqueSlug
  {
    get => (string)Data[nameof(UniqueSlug)]!;
    private set => Data[nameof(UniqueSlug)] = value;
  }

  public UniqueSlugAlreadyUsedException(Slug uniqueSlug) : base(BuildMessage(uniqueSlug))
  {
    UniqueSlug = uniqueSlug.Value;
  }

  private static string BuildMessage(Slug uniqueSlug) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UniqueSlug), uniqueSlug)
    .Build();
}
