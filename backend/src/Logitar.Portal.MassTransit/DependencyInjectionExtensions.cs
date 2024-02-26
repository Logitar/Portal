using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal.MassTransit;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarPortalMassTransit(this IServiceCollection services)
  {
    return services
      .AddMassTransit(x =>
      {
        x.UsingRabbitMq((context, config) =>
        {
          config.Host(host: "localhost", port: 5677, virtualHost: "/", h =>
          {
            h.Username(username: "guest");
            h.Password(password: "guest");
          });
          config.ConfigureEndpoints(context);
        });
      });
  }
}
