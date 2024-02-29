﻿using Logitar.Net.Http;
using Logitar.Portal.Client;
using Logitar.Portal.Client.Configurations;
using Logitar.Portal.Contracts.Configurations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Logitar.Portal.Configurations;

internal class InitializeConfigurationTests : IClientTests
{
  private const string Path = "/api/configuration/initialize";
  private static Uri UriPath => new(Path, UriKind.Relative);

  private readonly JsonApiClient _client;
  private readonly BasicCredentials _credentials;
  private readonly JsonSerializerOptions _serializerOptions;

  public InitializeConfigurationTests(HttpClient client, IPortalSettings portalSettings)
  {
    HttpApiSettings apiSettings = new();
    if (!string.IsNullOrWhiteSpace(portalSettings.BaseUrl))
    {
      apiSettings.BaseUri = new Uri(portalSettings.BaseUrl.Trim(), UriKind.Absolute);
    }
    _client = new JsonApiClient(client, apiSettings);

    _credentials = portalSettings.Basic ?? throw new ArgumentException($"The basic credentials '{nameof(portalSettings.Basic)}' are required.", nameof(portalSettings));

    _serializerOptions = new();
    _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    _serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  }

  public async Task<bool> ExecuteAsync(TestContext context)
  {
    try
    {
      context.SetName(typeof(ConfigurationClient), nameof(ConfigurationClient.InitializeAsync));
      UserPayload user = new(_credentials.Username, _credentials.Password);
      InitializeConfigurationPayload payload = new(user, new SessionPayload())
      {
        DefaultLocale = "fr"
      };
      JsonRequestOptions options = new(payload)
      {
        SerializerOptions = _serializerOptions
      };
      _ = await _client.PostAsync<CurrentUser>(UriPath, options, context.CancellationToken);
      context.Succeed();
    }
    catch (Exception exception)
    {
      context.Fail(exception);
    }

    return !context.HasFailed;
  }
}
