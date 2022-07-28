using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Portal.Web
{
  internal static class OpenApiExtensions
  {
    public static IServiceCollection AddOpenApi(this IServiceCollection services)
    {
      ArgumentNullException.ThrowIfNull(services);

      return services.AddSwaggerGen(config =>
      {
        config.AddSecurity();
        config.SwaggerDoc(name: "v1", new OpenApiInfo
        {
          Contact = new OpenApiContact
          {
            Email = "francispion@hotmail.com",
            Name = "Francis Pion",
            Url = new Uri("https://www.francispion.ca/")
          },
          Description = "Identity management Web API.",
          License = new OpenApiLicense
          {
            Name = "Use under MIT",
            Url = new Uri("https://github.com/Utar94/Portal/blob/main/LICENSE")
          },
          Title = "Portal API",
          Version = "v1.0"
        });
      });
    }

    public static void UseOpenApi(this WebApplication application)
    {
      application.UseSwagger();
      application.UseSwaggerUI(config => config.SwaggerEndpoint("/swagger/v1/swagger.json", "Portal API v1.0"));
    }

    private static void AddSecurity(this SwaggerGenOptions options)
    {
      options.AddSecurityDefinition(Constants.Schemes.ApiKey, new OpenApiSecurityScheme
      {
        Description = "Enter your API key in the input below.",
        In = ParameterLocation.Header,
        Name = Constants.Headers.ApiKey,
        Scheme = Constants.Schemes.ApiKey,
        Type = SecuritySchemeType.ApiKey
      });
      options.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
        {
          new OpenApiSecurityScheme
          {
            In = ParameterLocation.Header,
            Name = Constants.Headers.ApiKey,
            Reference = new OpenApiReference
            {
              Id = Constants.Schemes.ApiKey,
              Type = ReferenceType.SecurityScheme
            },
            Scheme = Constants.Schemes.ApiKey,
            Type = SecuritySchemeType.ApiKey
          },
          new List<string>()
        }
      });

      options.AddSecurityDefinition(Constants.Schemes.Session, new OpenApiSecurityScheme
      {
        Description = "Enter your session ID in the input below.",
        In = ParameterLocation.Header,
        Name = Constants.Headers.Session,
        Scheme = Constants.Schemes.Session,
        Type = SecuritySchemeType.ApiKey
      });
      options.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
        {
          new OpenApiSecurityScheme
          {
            In = ParameterLocation.Header,
            Name = Constants.Headers.Session,
            Reference = new OpenApiReference
            {
              Id = Constants.Schemes.Session,
              Type = ReferenceType.SecurityScheme
            },
            Scheme = Constants.Schemes.Session,
            Type = SecuritySchemeType.ApiKey
          },
          new List<string>()
        }
      });
    }
  }
}
