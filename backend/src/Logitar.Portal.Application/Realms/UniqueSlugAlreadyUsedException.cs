using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms;

public class UniqueSlugAlreadyUsedException : Exception
{
  public const string ErrorMessage = "The specified unique slug is already used.";

  public Slug UniqueSlug
  {
    get => new((string)Data[nameof(UniqueSlug)]!);
    private set => Data[nameof(UniqueSlug)] = value.Value;
  }

  public UniqueSlugAlreadyUsedException(Slug uniqueSlug) : base(BuildMessage(uniqueSlug))
  {
    UniqueSlug = uniqueSlug;
  }

  private static string BuildMessage(Slug uniqueSlug) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UniqueSlug), uniqueSlug.Value)
    .Build();
}
