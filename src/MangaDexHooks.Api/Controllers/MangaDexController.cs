namespace MangaDexHooks.Api.Controllers;

[ApiController, /*Authorize*/]
public class MangaDexController : ControllerBase
{
	private readonly IMangaDex _md;
	private readonly IMdCacheService _cache;

	public MangaDexController(
		IMangaDex md, 
		IMdCacheService cache)
	{
		_md = md;
		_cache = cache;
	}

	private int CalcPage(int count, int size)
	{
		return (int)Math.Ceiling((double)count / size);
	}

	[HttpGet, Route("md/manga")]
	[ProducesResponseType(typeof(FailureResult), 400)]
	[ProducesDefaultResponseType(typeof(CollectionResult<Manga>))]
	public async Task<IActionResult> GetManga([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int size = 20)
	{
		if (size < 1 || size > 100) return BadRequest(ApiResults.Error("Invalid size. Must be between 1 and 100"));

		var result = await _md.Manga.List(new MangaFilter
		{
			Title = search ?? "",
			Limit = size,
			Offset = (page - 1) * size,
			Includes = new[] { MangaIncludes.artist, MangaIncludes.author, MangaIncludes.cover_art  }
		});
		if (result == null)
			return Ok(ApiResults.Collection(Array.Empty<Manga>(), 0, 0));

		return Ok(ApiResults.Collection(
			result.Data, 
			CalcPage(result.Total, size), 
			result.Total));
	}

	[HttpGet, Route("md/manga/{id}")]
	[ProducesResponseType(typeof(FailureResult), 404)]
	[ProducesDefaultResponseType(typeof(SuccessResult<Manga>))]
	public async Task<IActionResult> GetMangaFromId([FromRoute] string id)
	{
		var manga = await _cache.Manga(id);
		if (manga == null || manga.Data == null) return NotFound(ApiResults.NotFound("manga"));

		return Ok(ApiResults.Success(manga.Data));
	}

	[HttpGet, Route("md/group")]
	[ProducesResponseType(typeof(FailureResult), 400)]
	[ProducesDefaultResponseType(typeof(CollectionResult<ScanlationGroup>))]
	public async Task<IActionResult> GetGroup([FromQuery] string? search = null, [FromQuery] int page = 1, [FromQuery] int size = 20)
	{
		if (size < 1 || size > 100) return BadRequest(ApiResults.Error("Invalid size. Must be between 1 and 100"));

		var result = await _md.ScanlationGroup.List(new ScanlationGroupFilter
		{
			Name = search,
			Limit = size,
			Offset = (page - 1) * size
		});

		if (result == null)
			return Ok(ApiResults.Collection(Array.Empty<ScanlationGroup>(), 0, 0));

		return Ok(ApiResults.Collection(
			result.Data, 
			CalcPage(result.Total, size), 
			result.Total));
	}

	[HttpGet, Route("md/user/{id}")]
	[ProducesResponseType(typeof(FailureResult), 404)]
	[ProducesDefaultResponseType(typeof(SuccessResult<User>))]
	public async Task<IActionResult> GetUser([FromRoute] string id)
	{
		var result = await _cache.User(id);

		if (result == null)
			return NotFound(ApiResults.NotFound("MangaDex User"));

		return Ok(ApiResults.Success(result.Data));
	}

	[HttpGet, Route("md/list/{id}")]
	[ProducesResponseType(typeof(FailureResult), 404)]
	[ProducesDefaultResponseType(typeof(SuccessResult<CustomList>))]
	public async Task<IActionResult> GetList([FromRoute] string id)
	{
		var result = await _cache.List(id);

		if (result == null)
			return NotFound(ApiResults.NotFound("MangaDex Custom List"));

		return Ok(ApiResults.Success(result.Data));
	}
}
