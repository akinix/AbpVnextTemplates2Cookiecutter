using System.Xml;

public class BuildParameters
{

	public static BuildParameters Create(ICakeContext context, string tag)
	{
		// ���tag��Ϊnull ����Ϊ�˴����֧������tag��
		var isTagged = !string.IsNullOrEmpty(tag);
		context.Information($"{isTagged}");

		var buildParameters = new BuildParameters(context, isTagged);
		buildParameters.Initialize();
		return buildParameters;
	}

	public BuildParameters(ICakeContext context, bool isTagged)
	{
		Context = context;
		IsTagged = isTagged;
	}

	public ICakeContext Context { get; }
	public BuildVersion Version { get; private set; }
	public string Configuration { get; private set; }
	public bool IsTagged { get; private set; }
	public bool IsCI { get; private set; }
	public DirectoryPathCollection Projects { get; set; }
	public DirectoryPathCollection TestProjects { get; set; }
	public FilePathCollection ProjectFiles { get; set; }
	public FilePathCollection TestProjectFiles { get; set; }

	public string FullVersion()
	{
		return Version.VersionWithSuffix();
	}

	private void Initialize()
	{
		InitializeCore();
		InitializeVersion();
	}

	// 
	private void InitializeCore()
	{
		Projects = Context.GetDirectories("./src/*");
		TestProjects = Context.GetDirectories("./test/*");
		ProjectFiles = Context.GetFiles("./src/*/*.csproj");
		TestProjectFiles = Context.GetFiles("./test/*/*.csproj");

		var buildSystem = Context.BuildSystem();

		// �ж��Ƿ���CI�����
		IsCI = !buildSystem.IsLocalBuild;	

		Configuration = Context.Argument("Configuration", "Debug");
		if (IsCI)
		{
			Configuration = "Release";
		}
	}

	private void InitializeVersion()
	{
		// ��ȡversion.props�ļ�
		var versionFile = Context.File("./version.props");
		var content = System.IO.File.ReadAllText(versionFile.Path.FullPath);

		XmlDocument doc = new XmlDocument();
		doc.LoadXml(content);

		var versionMajor = doc.DocumentElement.SelectSingleNode("/Project/PropertyGroup/VersionMajor").InnerText;
		var versionMinor = doc.DocumentElement.SelectSingleNode("/Project/PropertyGroup/VersionMinor").InnerText;
		var versionPatch = doc.DocumentElement.SelectSingleNode("/Project/PropertyGroup/VersionPatch").InnerText;
		var versionQuality = doc.DocumentElement.SelectSingleNode("/Project/PropertyGroup/VersionQuality").InnerText;
		versionQuality = string.IsNullOrWhiteSpace(versionQuality) ? null : versionQuality;

		// Ĭ�Ϻ�׺ 
		// ���û��tag 
		//   ci���� ��ʾΪԤ���� ��׺ preview-ʱ���
		//   �������� ��ʾΪ������ ��׺ dv-ʱ���
		// �����Tag �����ɺ�׺ 
		var suffix = versionQuality;
		if (!IsTagged)
		{
			suffix += (IsCI ? "preview-" : "dv-") + Util.CreateStamp();
		}
		suffix = string.IsNullOrWhiteSpace(suffix) ? null : suffix;

		Version =
			new BuildVersion(int.Parse(versionMajor), int.Parse(versionMinor), int.Parse(versionPatch), versionQuality);
		Version.Suffix = suffix;
	}
}

public class BuildVersion
{
	// Major ���汾�� Minor �ΰ汾�� Patch ������
	public BuildVersion(int major, int minor, int patch, string quality)
	{
		Major = major;
		Minor = minor;
		Patch = patch;
		Quality = quality;
	}

	public int Major { get; set; }
	public int Minor { get; set; }
	public int Patch { get; set; }
	public string Quality { get; set; }
	public string Suffix { get; set; }

	public string VersionWithoutQuality()
	{
		return $"{Major}.{Minor}.{Patch}";
	}

	public string Version()
	{
		return VersionWithoutQuality() + (Quality == null ? string.Empty : $"-{Quality}");
	}

	public string VersionWithSuffix()
	{
		return Version() + (Suffix == null ? string.Empty : $"-{Suffix}");
	}
}
