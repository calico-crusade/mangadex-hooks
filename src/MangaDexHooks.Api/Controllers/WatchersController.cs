namespace MangaDexHooks.Api.Controllers;

[ApiController, Authorize]
public class WatchersController : ControllerBase
{
	private readonly IDbService _db;
	private readonly IWatcherService _watcher;

	public WatchersController(
		IDbService db, 
		IWatcherService watcher)
	{
		_db = db;
		_watcher = watcher;
	}

	[HttpGet, Route("webhook/{hookId}/watchers")]
	[ProducesResponseType(typeof(FailureResult), 401)]
	[ProducesDefaultResponseType(typeof(CollectionResult<DbWatcher>))]
	public async Task<IActionResult> Get([FromRoute] long hookId, [FromQuery] int page = 1, [FromQuery] int size = 100)
	{
		var pid = this.ProfileId();
		if (pid == null) return Unauthorized(ApiResults.Unauthorized);

		var item = await _db.Webhooks.Fetch(hookId);
		if (item == null) return NotFound(ApiResults.NotFound("Webhook"));
		if (item.OwnerId != pid.Value && !this.IsInRole(Roles.ADMIN)) return Unauthorized(ApiResults.Unauthorized);

		var results = await _db.Watchers.ByWebhook(hookId, page, size);
		return Ok(ApiResults.Success(results));
	}

	[HttpPost, Route("webhook/{hookId}/watchers")]
	[ProducesResponseType(typeof(FailureResult), 400)]
	[ProducesResponseType(typeof(FailureResult), 401)]
	[ProducesResponseType(typeof(FailureResult), 404)]
	[ProducesResponseType(typeof(FailureResult), 500)]
	[ProducesDefaultResponseType(typeof(UpsertResult))]
	public async Task<IActionResult> Post([FromRoute] long hookId, [FromBody] WatcherPost data)
	{
		var pid = this.ProfileId();
		if (pid == null) return Unauthorized(ApiResults.Unauthorized);

		var hook = await _db.Webhooks.Fetch(hookId);
		if (hook == null) return NotFound(ApiResults.NotFound("webhook"));
		if (hook.OwnerId != pid.Value && !this.IsInRole(Roles.ADMIN)) return Unauthorized(ApiResults.Unauthorized);

		var (resource, type) = data;
		var (code, message, outId) = await _watcher.Upsert(resource, type, hookId);

		if (code >= 200 && code < 300 && outId != null) return Ok(ApiResults.Created(outId.Value, "watcher"));

		return StatusCode(code, ApiResults.Error(code, message ?? "Something went wrong"));
	}

	[HttpDelete, Route("webhook/{hookId}/watchers/{id}")]
	[ProducesResponseType(typeof(FailureResult), 401)]
	[ProducesResponseType(typeof(FailureResult), 404)]
	[ProducesResponseType(typeof(FailureResult), 500)]
	[ProducesDefaultResponseType(typeof(SuccessResult))]
	public async Task<IActionResult> Delete([FromRoute] long hookId, [FromRoute] long id)
	{
		var pid = this.ProfileId();
		if (pid == null) return Unauthorized(ApiResults.Unauthorized);

		var hook = await _db.Webhooks.Fetch(hookId);
		if (hook == null) return NotFound(ApiResults.NotFound("webhook"));
		if (hook.OwnerId != pid.Value && !this.IsInRole(Roles.ADMIN)) return Unauthorized(ApiResults.Unauthorized);

		var item = await _db.Watchers.Fetch(id);
		if (item == null) return NotFound(ApiResults.NotFound("webhook watcher"));
		if (item.WebhookId != hook.Id) return Unauthorized(ApiResults.Unauthorized);

		await _db.Watchers.Delete(id);
		return Ok(ApiResults.Success());
	}
}
