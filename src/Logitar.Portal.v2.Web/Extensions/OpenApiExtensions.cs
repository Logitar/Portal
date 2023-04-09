using Logitar.Portal.v2.Core;
using Microsoft.OpenApi.Models;

namespace Logitar.Portal.v2.Web.Extensions;

public static class OpenApiExtensions
{
  private const string Title = "Portal API";

  public static IServiceCollection AddOpenApi(this IServiceCollection services)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(config =>
    {
      config.SwaggerDoc(name: $"v{Constants.Version.Major}", new OpenApiInfo
      {
        Contact = new OpenApiContact
        {
          Email = "francispion@hotmail.com",
          Name = "Logitar Team",
          Url = new Uri("https://github.com/Logitar/Portal", UriKind.Absolute)
        },
        Description = "Identity management system.",
        License = new OpenApiLicense
        {
          Name = "Use under MIT",
          Url = new Uri("https://github.com/Logitar/Portal/blob/main/LICENSE", UriKind.Absolute)
        },
        Title = Title,
        Version = $"v{Constants.Version}"
      });
    });

    return services;
  }

  public static void UseOpenApi(this IApplicationBuilder builder)
  {
    builder.UseSwagger();
    builder.UseSwaggerUI(config => config.SwaggerEndpoint(
      url: $"/swagger/v{Constants.Version.Major}/swagger.json",
      name: $"{Title} v{Constants.Version}"
    ));
  }
}
