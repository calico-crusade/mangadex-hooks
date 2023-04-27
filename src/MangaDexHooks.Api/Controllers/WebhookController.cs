namespace MangaDexHooks.Api.Controllers;

[ApiController, Authorize]
public class WebhookController : ControllerBase
{
	private const string DEFAULT_CHAPTER = "191bffdc-75ba-4de0-b8be-a23e3ddbc05f";
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
		chapterId ??= DEFAULT_CHAPTER;

		var pid = this.ProfileId();
		if (pid == null) return Unauthorized(ApiResults.Unauthorized);

		var item = await _db.Webhooks.Fetch(id);
		if (item == null) return NotFound(ApiResults.NotFound("webhook"));
		if (item.OwnerId != pid.Value && !this.IsInRole(Roles.ADMIN)) return Unauthorized(ApiResults.Unauthorized);

		var (msg, code) = await _api.Execute(item, chapterId);
		return Ok(ApiResults.Message(code, msg));
	}

	[HttpGet, Route("webhook/{id}/history")]
	[ProducesDefaultResponseType(typeof(CollectionResult<DbWebhookResult>))]
	[ProducesResponseType(typeof(FailureResult), 401)]
	[ProducesResponseType(typeof(FailureResult), 404)]
	public async Task<IActionResult> History([FromRoute] long id, [FromQuery] int page = 1, [FromQuery] int size = 100)
	{
		var pid = this.ProfileId();
		if (pid == null) return Unauthorized(ApiResults.Unauthorized);

		var item = await _db.Webhooks.Fetch(id);
		if (item == null) return NotFound(ApiResults.NotFound("webhook"));
		if (item.OwnerId != pid.Value && !this.IsInRole(Roles.ADMIN)) return Unauthorized(ApiResults.Unauthorized);

		var results = await _db.Results.ByWebhook(id, page, size);
		return Ok(ApiResults.Success(results));
	}

	[HttpGet, Route("webhook/history")]
	[ProducesDefaultResponseType(typeof(CollectionResult<ResultsWithHook>))]
	[ProducesResponseType(typeof(FailureResult), 401)]
	public async Task<IActionResult> HistoryByOwner([FromQuery] int page = 1, [FromQuery] int size = 100)
	{
		var pid = this.ProfileId();
		if (pid == null) return Unauthorized(ApiResults.Unauthorized);

		var item = await _db.Results.ByOwner(pid.Value, page, size);
		return Ok(ApiResults.Success(item));
	}

	[HttpGet, Route("webhook/shim"), AllowAnonymous]
	public async Task<IActionResult> TypeShim([FromQuery] bool wrap = false)
	{
		var shim = await Core.Extensions.ReadStaticFile("Scripts", "TemplateShim.d.ts");
		var defa = await Core.Extensions.ReadStaticFile("Scripts", "Defaultscript.js");
		if (string.IsNullOrEmpty(shim)) return NotFound();

		if (wrap)
			return Ok(ApiResults.Success(new ShimResult
			{
				Shim = shim,
				Default = defa ?? string.Empty
			}));

		return File(Encoding.UTF8.GetBytes(shim), "application/typescript");
	}

	[HttpPost, Route("webhook/test")]
	[ProducesDefaultResponseType(typeof(SuccessResult<Webhook>))]
	[ProducesResponseType(typeof(FailureResult), 500)]
	[ProducesResponseType(typeof(FailureResult), 404)]
	[ProducesResponseType(typeof(FailureResult), 400)]
	public async Task<IActionResult> TestLocal([FromBody] WebhookTest test)
	{
		try
		{
			var (manga, chapter, cover) = await _api.GetData(test.ChapterId ?? DEFAULT_CHAPTER);
			if (manga == null || chapter == null) return NotFound(ApiResults.NotFound("MangaDex Chapter"));

			var (hook, error, code) = await _api.ExecuteWebhookScript(test.Script, chapter, manga, cover);
			if (hook == null || !string.IsNullOrEmpty(error))
				return StatusCode(code, ApiResults.Error(code, error ?? "Something went wrong while running your script"));

			return Ok(ApiResults.Success(hook));
		}
		catch (Exception ex)
		{
			return StatusCode(500, ApiResults.Error(ex));
		}
	}
}
