using FluentValidation;
using Portal.Core.Realms;

namespace Portal.Core.Users
{
  internal static class ValidationContextExtensions
  {
    private const string AllowedUsernameCharactersKey = nameof(Realm.AllowedUsernameCharacters);

    public static string? GetAllowedUsernameCharacters<T>(this ValidationContext<T> context)
    {
      ArgumentNullException.ThrowIfNull(context);

      return context.RootContextData.TryGetValue(AllowedUsernameCharactersKey, out object? value)
        ? (string)value
        : null;
    }

    public static void SetAllowedUsernameCharacters<T>(this ValidationContext<T> context, string? allowedUsernameCharacters)
    {
      ArgumentNullException.ThrowIfNull(context);

      if (allowedUsernameCharacters != null)
      {
        context.RootContextData[AllowedUsernameCharactersKey] = allowedUsernameCharacters;
      }
    }
  }
}
