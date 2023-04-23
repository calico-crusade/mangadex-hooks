using System.Net;

namespace MangaDexHooks.Api;

public class ApiResult
{
	[JsonPropertyName("result")]
	public string Result { get; set; } = string.Empty;

	[JsonPropertyName("code")]
	public int Code { get; set; }

	public ApiResult() { }

	public ApiResult(int code, string result)
	{
		Code = code;
		Result = result;
	}

	public ApiResult(HttpStatusCode code, string result) : this((int)code, result) { }
}

public class SuccessResult : ApiResult
{
	public SuccessResult() : base(HttpStatusCode.OK, "ok") { }
}

public class SuccessResult<T> : SuccessResult
{
	[JsonPropertyName("data")]
	public T Data { get; set; }

	public SuccessResult(T data) : base()
	{
		Data = data;
	}
}

public class CollectionResult<T> : SuccessResult<T[]>
{
	[JsonPropertyName("pages")]
	public int Pages { get; set; }

	[JsonPropertyName("count")]
	public int Count { get; set; }

	public CollectionResult(PaginatedResult<T> results) : base(results.Results)
	{
		Pages = results.Pages;
		Count = results.Count;
	}
}

public class FailureResult : ApiResult
{
	[JsonPropertyName("errors")]
	public string[] Errors { get; set; } = Array.Empty<string>();

	public FailureResult() { }

	public FailureResult(int code, params string[] errors) : base(code, "error")
	{
		Errors = errors;
	}

	public FailureResult(int code, IEnumerable<string> errors): base(code, "error")
	{
		Errors = errors.ToArray();
	}

	public FailureResult(HttpStatusCode code, params string[] errors): base(code, "error")
	{
		Errors = errors;
	}

	public FailureResult(HttpStatusCode code, IEnumerable<string> errors) : base(code, "error")
	{
		Errors = errors.ToArray();
	}
}

public class UpsertResult : SuccessResult
{
	[JsonPropertyName("id")]
	public long Id { get; set; }

	[JsonPropertyName("resource")]
	public string Resource { get; set; } = string.Empty;

	public UpsertResult() { }

	public UpsertResult(long id, string resource) : base()
	{
		Id = id;
		Resource = resource;
	}
}

public static class ApiResults
{
	public static SuccessResult Empty => new();

	public static SuccessResult<T> Success<T>(T data) => new(data);

	public static CollectionResult<T> Success<T>(PaginatedResult<T> results) => new(results);

	public static FailureResult Error(int code, params string[] errors) => new(code, errors);

	public static FailureResult Error(HttpStatusCode code, params string[] errors) => new(code, errors);

	public static FailureResult Error(params string[] errors) => new(HttpStatusCode.InternalServerError, errors);
	
	public static FailureResult Error(params Exception[] exceptions) => Error(exceptions.Select(t => t.Message).ToArray());

	public static FailureResult Unauthorized => new(HttpStatusCode.Unauthorized, "You do not have the right permissions to access this.");

	public static FailureResult NotFound(string resource) => new(HttpStatusCode.NotFound, $"I couldn't find that {resource}.");

	public static UpsertResult Created(long id, string resource) => new(id, resource);
}