using Microsoft.OpenApi.Models;

namespace Logitar.Portal.Extensions;

public static class OpenApiExtensions
{
  private const string Title = "Portal API";

  private static readonly Version Version = new(3, 0, 0);

  public static IServiceCollection AddOpenApi(this IServiceCollection services)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
      options.SwaggerDoc(name: $"v{Version.Major}", new OpenApiInfo
      {
        Contact = new OpenApiContact
        {
          Email = "francispion@hotmail.com",
          Name = "Logitar Team",
          Url = new Uri("https://github.com/Logitar/Portal/")
        },
        Description = "Identity provider system.",
        License = new OpenApiLicense
        {
          Name = "Use under MIT",
          Url = new Uri("https://github.com/Logitar/Portal/blob/main/LICENSE")
        },
        Title = Title,
        Version = $"v{Version}"
      });
    });

    return services;
  }

  public static void UseOpenApi(this IApplicationBuilder builder)
  {
    builder.UseSwagger();
    builder.UseSwaggerUI(config => config.SwaggerEndpoint(
      url: $"/swagger/v{Version.Major}/swagger.json",
      name: $"{Title} v{Version}"
    ));
  }
}
