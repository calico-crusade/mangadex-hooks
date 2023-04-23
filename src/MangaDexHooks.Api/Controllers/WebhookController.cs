namespace MangaDexHooks.Api.Controllers;

[ApiController, Authorize]
public class WebhookController : ControllerBase
{
	private readonly IDbService _db;
	private readonly IWebhookApiService _api;

	public WebhookController(
		IDbService db, 
		IWebhookApiService api)
	{
		_db = db;
		_api = api;
	}

	[HttpGet, Route("webhook")]
	[ProducesDefaultResponseType(typeof(CollectionResult<DbWebhook>))]
	[ProducesResponseType(typeof(FailureResult), 401)]
	public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 100)
	{
		var pid = this.ProfileId();
		if (pid == null) return Unauthorized(ApiResults.Unauthorized);

		var results = await _db.Webhooks.ByOwner(pid.Value, page, size);
		return Ok(ApiResults.Success(results));
	}

	[HttpGet, Route("webhook/{id}")]
	[ProducesDefaultResponseType(typeof(SuccessResult<DbWebhook>))]
	[ProducesResponseType(typeof(FailureResult), 404)]
	[ProducesResponseType(typeof(FailureResult), 401)]
	public async Task<IActionResult> Get([FromRoute]long id)
	{
		var pid = this.ProfileId();
		if (pid == null) return Unauthorized(ApiResults.Unauthorized);

		var item = await _db.Webhooks.Fetch(id);
		if (item == null) return NotFound(ApiResults.NotFound("webhook"));

		if (item.OwnerId != pid.Value && !this.IsInRole(Roles.ADMIN)) 
			return Unauthorized(ApiResults.Unauthorized); 

		return Ok(ApiResults.Success(item));
	}

	[HttpPost, Route("webhook")]
	[ProducesDefaultResponseType(typeof(CreatedResult))]
	[ProducesResponseType(typeof(FailureResult), 401)]
	public async Task<IActionResult> Post([FromBody] DbWebhook webhook)
	{
		var pid = this.ProfileId();
		if (pid == null) return Unauthorized(ApiResults.Unauthorized);

		var item = await _db.Webhooks.Fetch(webhook.Id);
		if (item != null && item.OwnerId != pid.Value && !this.IsInRole(Roles.ADMIN)) 
			return Unauthorized(ApiResults.Unauthorized);

		webhook.OwnerId = pid.Value;

		var id = await _db.Webhooks.Upsert(webhook);
		return Ok(ApiResults.Created(id, "webhook"));
	}

	[HttpGet, Route("webhook/{id}/test")]
	[ProducesDefaultResponseType(typeof(ApiResult))]
	[ProducesResponseType(typeof(FailureResult), 401)]
	[ProducesResponseType(typeof(FailureResult), 404)]
	public async Task<IActionResult> Test([FromRoute] long id, [FromQuery] string? chapterId = null)
	{
		chapterId ??= "191bffdc-75ba-4de0-b8be-a23e3ddbc05f";

		var pid = this.ProfileId();
		if (pid == null) return Unauthorized(ApiResults.Unauthorized);

		var item = await _db.Webhooks.Fetch(id);
		if (item == null) return NotFound(ApiResults.NotFound("webhook"));
		if (item.OwnerId != pid.Value && !this.IsInRole(Roles.ADMIN)) return Unauthorized(ApiResults.Unauthorized);

		var (msg, code) = await _api.Execute(item, chapterId);
		return Ok(new ApiResult(code, msg));
	}
}
