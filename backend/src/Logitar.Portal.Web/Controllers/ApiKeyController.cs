using Logitar.Portal.Application.ApiKeys;
using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Web.Extensions;
using Logitar.Portal.Web.Models.ApiKeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logitar.Portal.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/keys")]
public class ApiKeyController : ControllerBase
{
  private readonly IApiKeyService _apiKeyService;

  public ApiKeyController(IApiKeyService apiKeyService)
  {
    _apiKeyService = apiKeyService;
  }

  [HttpPatch("authenticate")]
  public async Task<ActionResult<ApiKeyModel>> AuthenticateAsync([FromBody] AuthenticateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    return Ok(await _apiKeyService.AuthenticateAsync(payload, cancellationToken));
  }

  [HttpPost]
  public async Task<ActionResult<ApiKeyModel>> CreateAsync([FromBody] CreateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKeyModel apiKey = await _apiKeyService.CreateAsync(payload, cancellationToken);
    return Created(BuildLocation(apiKey), apiKey);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<ApiKeyModel>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    ApiKeyModel? apiKey = await _apiKeyService.DeleteAsync(id, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ApiKeyModel>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ApiKeyModel? apiKey = await _apiKeyService.ReadAsync(id: id, cancellationToken: cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<ApiKeyModel>> ReplaceAsync(Guid id, [FromBody] ReplaceApiKeyPayload payload, long? version, CancellationToken cancellationToken)
  {
    ApiKeyModel? apiKey = await _apiKeyService.ReplaceAsync(id, payload, version, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<ApiKeyModel>>> SearchAsync([FromQuery] SearchApiKeysModel model, CancellationToken cancellationToken)
  {
    SearchApiKeysPayload payload = model.ToPayload();
    return Ok(await _apiKeyService.SearchAsync(payload, cancellationToken));
  }

  [HttpPatch("{id}")]
  public async Task<ActionResult<ApiKeyModel>> UpdateAsync(Guid id, [FromBody] UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKeyModel? apiKey = await _apiKeyService.UpdateAsync(id, payload, cancellationToken);
    return apiKey == null ? NotFound() : Ok(apiKey);
  }

  private Uri BuildLocation(ApiKeyModel apiKey) => HttpContext.BuildLocation("keys/{id}", new Dictionary<string, string> { ["id"] = apiKey.Id.ToString() });
}
