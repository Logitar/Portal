using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Constants;
using Logitar.Portal.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.PortalActor)]
[Route("api/keys")]
public class ApiKeyController : ControllerBase
{
  private readonly IApiKeyService _apiKeyService;

  public ApiKeyController(IApiKeyService apiKeyService)
  {
    _apiKeyService = apiKeyService;
  }

  [HttpPatch("authenticate/{xApiKey}")]
  public async Task<ActionResult<User>> AuthenticateAsync(string xApiKey, CancellationToken cancellationToken)
  {
    return Ok(await _apiKeyService.AuthenticateAsync(xApiKey, cancellationToken));
  }

  [HttpPost]
  public async Task<ActionResult<ApiKey>> CreateAsync([FromBody] CreateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKey apiKey = await _apiKeyService.CreateAsync(payload, cancellationToken);
    Uri uri = new($"{Request.Scheme}://{Request.Host}/api/keys/{apiKey.Id}");

    return Created(uri, apiKey);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<ApiKey>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.DeleteAsync(id, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ApiKey>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.ReadAsync(id, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ApiKey>> ReplaceAsync(Guid id, [FromBody] ReplaceApiKeyPayload payload, long? version, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.ReplaceAsync(id, payload, version, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpPost("search")]
  public async Task<ActionResult<SearchResults<ApiKey>>> SearchAsync([FromBody] SearchApiKeysPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _apiKeyService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ApiKey>> UpdateAsync(Guid id, [FromBody] UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.UpdateAsync(id, payload, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }
}
