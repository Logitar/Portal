using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Extensions;
using Logitar.Portal.Models.ApiKeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Controllers;

[ApiController]
[Authorize]
[Route("api-keys")]
public class ApiKeyController : ControllerBase
{
  private readonly IApiKeyService _apiKeyService;

  public ApiKeyController(IApiKeyService apiKeyService)
  {
    _apiKeyService = apiKeyService;
  }

  [HttpPatch("authenticate")]
  public async Task<ActionResult<ApiKey>> AuthenticateAsync([FromBody] AuthenticateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _apiKeyService.AuthenticateAsync(payload, cancellationToken));
  }

  [HttpPost]
  public async Task<ActionResult<ApiKey>> CreateAsync([FromBody] CreateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKey apiKey = await _apiKeyService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(apiKey), apiKey);
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
    ApiKey? apiKey = await _apiKeyService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ApiKey>> ReplaceAsync(Guid id, [FromBody] ReplaceApiKeyPayload payload, long? version, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.ReplaceAsync(id, payload, version, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ApiKey>>> SearchAsync([FromQuery] SearchApiKeysModel model, CancellationToken cancellationToken)
  {
    SearchApiKeysPayload payload = model.ToPayload();
    return Ok(await _apiKeyService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ApiKey>> UpdateAsync(Guid id, [FromBody] UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await _apiKeyService.UpdateAsync(id, payload, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  private Uri BuildLocation(ApiKey apiKey) => HttpContext.BuildLocation("keys/{id}", new Dictionary<string, string> { ["id"] = apiKey.Id.ToString() });
}
