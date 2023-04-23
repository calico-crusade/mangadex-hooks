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
	[ProducesDefaultResponseType(typeof(SuccessResult<UserLoginResult>))]
	[ProducesResponseType(typeof(FailureResult), 401)]
	[ProducesResponseType(typeof(FailureResult), 500)]
	public async Task<IActionResult> Auth(string code)
	{
		var res = await _auth.ResolveCode(code);
		if (res == null || !string.IsNullOrEmpty(res.Error))
			return Unauthorized(ApiResults.Error(401, "Invalid authorization code"));

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
			return StatusCode(500, ApiResults.Error("Something went wrong while fetching your profile."));
		}

		var user = (UserResult)profile;
		var token = _token.GenerateToken((Claim[])user);

		return Ok(ApiResults.Success(new UserLoginResult
		{
			Token = token,
			User = user
		}));
	}

	[HttpGet, Route("auth"), Authorize]
	[ProducesDefaultResponseType(typeof(SuccessResult<UserResult>))]
	public IActionResult Me()
	{
		var claims = (UserResult)User.Claims.ToArray();
		if (claims.ProfileId == 0) return Unauthorized();

		return Ok(ApiResults.Success(claims));
	}

	[HttpGet, Route("auth/url")]
	[ProducesDefaultResponseType(typeof(SuccessResult<string>))]
	public IActionResult AuthUrl([FromQuery] string? redirect = null)
	{
		var url = $"{_auth.Url}/Home/Auth/{_auth.AppId}";
		if (!string.IsNullOrEmpty(redirect))
			url += "?redirect=" + redirect.UrlEncode();

		return Ok(ApiResults.Success(url));
	}
}