//using Logitar.Portal.Application.Realms;
//using Logitar.Portal.Constants;
//using Logitar.Portal.Contracts.Constants;
//using Logitar.Portal.Contracts.Errors;
//using Logitar.Portal.Contracts.Realms;
//using Logitar.Portal.Extensions;
//using Logitar.Portal.Web.Extensions;
//using Microsoft.Extensions.Primitives;

//namespace Logitar.Portal.Middlewares;

//internal class ResolveRealm
//{
//  private readonly RequestDelegate _next;

//  public ResolveRealm(RequestDelegate next)
//  {
//    _next = next;
//  }

//  public async Task InvokeAsync(HttpContext context, IConfiguration configuration, IRealmService realmService)
//  {
//    if (context.Request.Headers.TryGetValue(Headers.Realm, out StringValues headerValues))
//    {
//      IReadOnlyCollection<string> values = headerValues.ExtractValues();
//      if (values.Count > 0)
//      {
//        if (context.GetApiKey() == null && context.GetUser() == null)
//        {
//          context.Response.Headers.WWWAuthenticate = string.Join(", ", Schemes.GetEnabled(configuration));
//          Error error = new("AuthenticatedActorRequired", "An authenticated actor is required to manage realm objects.");
//          await context.SetResponseAsync(StatusCodes.Status401Unauthorized, error);
//          return;
//        }
//        else if (values.Count > 1)
//        {
//          Error error = new("TooManyRealms", $"The '{Headers.Realm}' header only supports one value.");
//          await context.SetResponseAsync(StatusCodes.Status400BadRequest, error);
//          return;
//        }

//        string value = values.Single();
//        bool parsed = Guid.TryParse(value, out Guid id);
//        Realm? realm = await realmService.ReadAsync(id: parsed ? id : null, uniqueSlug: value);
//        if (realm == null)
//        {
//          Error error = new("RealmNotFound", $"The realm '{value}' could not be found.");
//          await context.SetResponseAsync(StatusCodes.Status404NotFound, error);
//          return;
//        }

//        context.SetRealm(realm);
//      }
//    }

//    await _next(context);
//  }
//}
