using Logitar.Portal.ApiKeys;
using Logitar.Portal.Client;
using Logitar.Portal.Client.ApiKeys;
using Logitar.Portal.Configurations;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Dictionaries;
using Logitar.Portal.Messages;
using Logitar.Portal.OneTimePasswords;
using Logitar.Portal.Realms;
using Logitar.Portal.Roles;
using Logitar.Portal.Senders;
using Logitar.Portal.Sessions;
using Logitar.Portal.Templates;
using Logitar.Portal.Tokens;
using Logitar.Portal.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Portal;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public virtual void ConfigureServices(IServiceCollection services)
  {
    PortalSettings settings = _configuration.GetSection("Portal").Get<PortalSettings>() ?? new();
    StaticPortalSettings.Instance.BaseUrl = settings.BaseUrl;
    StaticPortalSettings.Instance.Basic = settings.Basic;
    services.AddLogitarPortalClient(StaticPortalSettings.Instance);
    services.AddTransient<IApiKeyClient, ApiKeyClient>(); // NOTE(fpion): required since we inject it two times to create a Portal API key.

    services.AddTransient<CreateApiKeyTests>();
    services.AddTransient<DeleteApiKeyTests>();
    services.AddTransient<DeleteRealmTests>();

    services.AddTransient<ApiKeyClientTests>();
    services.AddTransient<ConfigurationClientTests>();
    services.AddTransient<DictionaryClientTests>();
    services.AddTransient<MessageClientTests>();
    services.AddTransient<OneTimePasswordClientTests>();
    services.AddTransient<RealmClientTests>();
    services.AddTransient<RoleClientTests>();
    services.AddTransient<SenderClientTests>();
    services.AddTransient<SessionClientTests>();
    services.AddTransient<TemplateClientTests>();
    services.AddTransient<TokenClientTests>();
    services.AddTransient<UserClientTests>();
  }
}
