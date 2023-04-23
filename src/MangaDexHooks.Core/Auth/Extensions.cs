using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MangaDexHooks.Core.Auth;

public static class Extensions
{
	public static string? GroupId(this ClaimsPrincipal principal) => principal.Claim(ClaimTypes.GroupSid);

	public static string? GroupId(this ControllerBase ctrl) => ctrl?.User?.GroupId();

	public static string? Claim(this ClaimsPrincipal principal, string claim)
	{
		return principal?.FindFirst(claim)?.Value;
	}

	public static string? Claim(this ControllerBase ctrl, string claim)
	{
		if (ctrl.User == null) return null;
		return ctrl.User.Claim(claim);
	}

	public static long? ProfileId(this ControllerBase ctrl)
	{
		var (pid, _) = ctrl.UserFromIdentity();
		return pid;
	}

	public static bool IsInRole(this ControllerBase ctrl, string role)
	{
		if (ctrl.User == null) return false;
		return ctrl.User.IsInRole(role);
	}

	public static (long? pid, TokenUser? user) UserFromIdentity(this ControllerBase ctrl)
	{
		if (ctrl.User == null) return (null, null);

		return ctrl.User.UserFromIdentity();
	}

	public static (long? pid, TokenUser? user) UserFromIdentity(this ClaimsPrincipal principal)
	{
		if (principal == null) return (null, null);

		var getClaim = (string key) => principal.Claim(key) ?? "";

		var id = getClaim(ClaimTypes.NameIdentifier);
		var pid = getClaim(ClaimTypes.Uri);
		if (string.IsNullOrEmpty(id) ||
			!long.TryParse(pid, out long profileId)) return (null, null);

		return (profileId, new TokenUser
		{
			Id = id,
			Nickname = getClaim(ClaimTypes.Name),
			Email = getClaim(ClaimTypes.Email),
			Avatar = getClaim(ClaimTypes.UserData),
			Provider = getClaim(ClaimTypes.PrimarySid),
			ProviderId = getClaim(ClaimTypes.PrimaryGroupSid)
		});
	}
}
