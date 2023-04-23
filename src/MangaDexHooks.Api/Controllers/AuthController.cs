using System.Security.Claims;

namespace MangaDexHooks.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
	private readonly IDbService _db;
	private readonly IOAuthService _auth;
	private readonly ITokenService _token;
	private readonly ILogger _logger;

	public AuthController(
		IDbService db,
		IOAuthService auth,
		ITokenService token,
		ILogger<AuthController> logger)
	{
		_db = db;
		_auth = auth;
		_token = token;
		_logger = logger;
	}

	[HttpGet, Route("auth/{code}")]
	public async Task<IActionResult> Auth(string code)
	{
		var res = await _auth.ResolveCode(code);
		if (res == null || !string.IsNullOrEmpty(res.Error))
			return Unauthorized(new
			{
				error = res?.Error ?? "Login Failed"
			});

		var profile = new DbProfile
		{
			Avatar = res.User.Avatar,
			Email = res.User.Email,
			PlatformId = res.User.Id,
			Username = res.User.Nickname,
			Provider = res.User.Provider,
			ProviderId = res.User.ProviderId,
		};
		await _db.Profiles.Upsert(profile);

		profile = await _db.Profiles.Fetch(res.User.Id);

		if (profile == null)
		{
			_logger.LogError("Could not find profile for newly upserted object: {profile}", profile.JsonSerialize());
			return StatusCode(500, new
			{
				error = "An error occurred while generating profile"
			});
		}

		var roles = profile.Admin ? new[] { "Admin" } : Array.Empty<string>();
		var token = _token.GenerateToken(res, roles);

		return Ok(new
		{
			user = new
			{
				roles,
				nickname = res.User.Nickname,
				avatar = res.User.Avatar,
				id = res.User.Id,
				email = res.User.Email
			},
			token,
			id = profile.Id
		});
	}

	[HttpGet, Route("auth"), Authorize]
	public IActionResult Me()
	{
		var user = this.UserFromIdentity();
		if (user == null) return Unauthorized();

		var roles = User.Claims.Where(t => t.Type == ClaimTypes.Role).Select(t => t.Value).ToArray();

		return Ok(new
		{
			roles,
			nickname = user.Nickname,
			avatar = user.Avatar,
			id = user.Id,
			email = user.Email
		});
	}

	[HttpGet, Route("auth/url")]
	public IActionResult AuthUrl([FromQuery] string? redirect = null)
	{
		var url = $"{_auth.Url}/Home/Auth/{_auth.AppId}";
		if (!string.IsNullOrEmpty(redirect))
			url += "?redirect=" + redirect.UrlEncode();

		return Ok(new
		{
			url
		});
	}
}