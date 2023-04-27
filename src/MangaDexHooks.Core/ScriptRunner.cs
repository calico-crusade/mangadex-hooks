using Jint;
using Jint.Native;

namespace MangaDexHooks.Core;

public class ScriptRunner : IDisposable
{
	private readonly Engine _engine;
	private readonly string _script;

	public string? Output { get; private set; }
	public JsValue? Result { get; private set; }

	public ScriptRunner(string script, int timeoutSec = 10, int recursion = 900)
	{
		_engine = new Engine(c =>
		{
			c.TimeoutInterval(TimeSpan.FromSeconds(timeoutSec))
			 .LimitRecursion(recursion);
		});
		_script = script;
	}

	public void SetOutput(string output)
	{
		Output = output;
	}

	public uint ToColor(int r, int g, int b)
	{
		return new Color(r, g, b).RawValue;
	}

	public string? Eval(Manga manga, Chapter chapter, string? cover)
	{
		_engine
			.SetValue("setOutput", SetOutput)
			.SetValue("MangaString", manga.JsonSerialize() ?? string.Empty)
			.SetValue("ChapterString", chapter.JsonSerialize() ?? string.Empty)
			.SetValue("CoverUrlString", cover ?? string.Empty)
			.SetValue("ToColor", ToColor);

		Result = _engine.Evaluate(_script);

		return Output;
	}

	public void Dispose()
	{
		_engine.Dispose();
	}
}
