namespace MangaDexHooks.Api.Controllers;

[ApiController, Authorize]
public class WebhookController : ControllerBase
{
	private readonly IDbService _db;

	public WebhookController(IDbService db)
	{
		_db = db;
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

		var id = await _db.Webhooks.Upsert(webhook);
		return Ok(ApiResults.Created(id, "webhook"));
	}
}
