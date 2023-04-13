using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Logitar.Portal.Web.Extensions;

public static class OpenApiExtensions
{
  private const string Title = "Portal API";

  public static IServiceCollection AddOpenApi(this IServiceCollection services)
  {
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(config =>
    {
      config.AddSecurity();
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

  private static void AddSecurity(this SwaggerGenOptions options)
  {
    options.AddSecurityDefinition(Constants.Schemes.Basic, new OpenApiSecurityScheme
    {
      Description = "Enter your credentials in the inputs below:",
      In = ParameterLocation.Header,
      Name = Constants.Headers.Authorization,
      Scheme = Constants.Schemes.Basic,
      Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
      {
        new OpenApiSecurityScheme
        {
          In = ParameterLocation.Header,
          Name = Constants.Headers.Authorization,
          Reference = new OpenApiReference
          {
            Id = Constants.Schemes.Basic,
            Type = ReferenceType.SecurityScheme
          },
          Scheme = Constants.Schemes.Basic,
          Type = SecuritySchemeType.Http
        },
        new List<string>()
      }
    });
  }
}
