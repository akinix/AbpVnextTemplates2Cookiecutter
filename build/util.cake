public class Util
{
	private readonly ICakeContext _context;
	private readonly BuildParameters _build;

	public Util(ICakeContext context, BuildParameters build)
	{
		_context = context;
		_build = build;
	}

	// ��������� �汾�źͱ���ѡ��
	public void PrintInfo()
	{
		_context.Information($@"
Version:       {_build.FullVersion()}
Configuration: {_build.Configuration}
");
	}

	public static string CreateStamp()
	{
		var seconds = (long)(DateTime.UtcNow - new DateTime(2017, 1, 1)).TotalSeconds;
		return seconds.ToString();
	}
}
