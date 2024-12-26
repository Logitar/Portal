using GraphQL;
using GraphQL.Types;
using Logitar.Portal.Application.Passwords;

namespace Logitar.Portal.GraphQL.Passwords;

internal static class OneTimePasswordQueries
{
  public static void Register(RootQuery root)
  {
    root.Field<OneTimePasswordGraphType>("oneTimePassword")
      .Authorize()
      .Description("Retrieves a single One-Time Password (OTP).")
      .Arguments(
        new QueryArgument<NonNullGraphType<IdGraphType>>() { Name = "id", Description = "The unique identifier of the One-Time Password (OTP)." }
      )
      .ResolveAsync(async context => await context.GetQueryService<IOneTimePasswordService, object?>().ReadAsync(
        context.GetArgument<Guid>("id"),
        context.CancellationToken
      ));
  }
}
