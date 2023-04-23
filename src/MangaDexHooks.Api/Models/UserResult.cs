using System.Security.Claims;

namespace MangaDexHooks.Api;

public class UserResult
{
	[JsonPropertyName("roles")]
	public string[] Roles { get; set; } = Array.Empty<string>();

	[JsonPropertyName("nickname")]
	public string Nickname { get; set; } = string.Empty;

	[JsonPropertyName("avatar")]
	public string Avatar { get; set; } = string.Empty;

	[JsonPropertyName("platformId")]
	public string PlatformId { get; set; } = string.Empty;

	[JsonPropertyName("provider")]
	public string Provider { get; set; } = string.Empty;

	[JsonPropertyName("providerId")]
	public string ProviderId { get; set; } = string.Empty;

	[JsonIgnore]
	public virtual string Email { get; set; } = string.Empty;

	[JsonPropertyName("profileId")]
	public long ProfileId { get; set; }

	public static implicit operator Claim[](UserResult user)
	{
		return new Claim[]
		{
			new(ClaimTypes.Uri, user.ProfileId.ToString()),
			new(ClaimTypes.NameIdentifier, user.PlatformId),
			new(ClaimTypes.Name, user.Nickname),
			new(ClaimTypes.Email, user.Email),
			new(ClaimTypes.UserData, user.Avatar),
			new(ClaimTypes.PrimarySid, user.Provider),
			new(ClaimTypes.PrimaryGroupSid, user.ProviderId),
			
		}.Concat(user.Roles.Select(t => new Claim(ClaimTypes.Role, t)))
		.ToArray();
	}

	public static implicit operator UserResult(Claim[] claims)
	{
		var getClaim = (string key) => claims.FirstOrDefault(t => t.Type.ToLower() == key.ToLower())?.Value ?? "";

		return new UserResult
		{
			ProfileId = long.TryParse(getClaim(ClaimTypes.Uri), out var id) ? id : 0,
			PlatformId = getClaim(ClaimTypes.NameIdentifier),
			Nickname = getClaim(ClaimTypes.Name),
			Email = getClaim(ClaimTypes.Email),
			Avatar = getClaim(ClaimTypes.UserData),
			Provider = getClaim(ClaimTypes.PrimarySid),
			ProviderId = getClaim(ClaimTypes.PrimaryGroupSid),
			Roles = claims.Where(t => t.Type == ClaimTypes.Role).Select(t => t.Value).ToArray()
		};
	}

	public static implicit operator UserResult(DbProfile profile) 
	{
		return new UserResult
		{
			Avatar = profile.Avatar,
			Email = profile.Email,
			ProfileId = profile.Id,
			Provider = profile.Provider,
			ProviderId = profile.ProviderId,
			Nickname = profile.Username,
			PlatformId = profile.PlatformId,
			Roles = profile.Admin ? new[] { Api.Roles.ADMIN } : Array.Empty<string>()
		};
	}
}

public class UserLoginResult
{
	[JsonPropertyName("user")]
	public UserResult User { get; set; } = new();

	[JsonPropertyName("token")]
	public string Token { get; set; } = string.Empty;
}