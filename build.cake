#addin nuget:?package=Newtonsoft.Json&version=9.0.1

#load "./build/util.cake"
#load "./build/version.cake"

// ./build.ps1 -ScriptArgs '-tag="v1.1"'
var target = Argument("target", "Default");
// 取命令行参数 默认值为空 防止报错
var tag = Argument("tag", "");

var build = BuildParameters.Create(Context, tag);
var util = new Util(Context, build);

Task("Clean")
	.Does(() =>
{
	if (DirectoryExists("./artifacts"))
	{
		DeleteDirectory("./artifacts", true);
	}
});

Task("Restore")
	.IsDependentOn("Clean")
	.Does(() =>
{
	var settings = new DotNetCoreRestoreSettings
	{
		ArgumentCustomization = args =>
		{
			args.Append($"/p:VersionSuffix={build.Version.Suffix}");
			return args;
		}
	};
	DotNetCoreRestore(settings);
});

Task("Build")
	.IsDependentOn("Restore")
	.Does(() =>
{
	var settings = new DotNetCoreBuildSettings
	{
		Configuration = build.Configuration,
		VersionSuffix = build.Version.Suffix,
		ArgumentCustomization = args =>
		{
			args.Append($"/p:InformationalVersion={build.Version.VersionWithSuffix()}");
			return args;
		}
	};
	foreach (var project in build.ProjectFiles)
	{
		DotNetCoreBuild(project.FullPath, settings);
	}
});

Task("Test")
	.IsDependentOn("Build")
	.Does(() =>
{
	foreach (var testProject in build.TestProjectFiles)
	{
		DotNetCoreTest(testProject.FullPath);
	}
});

Task("Pack")
	.Does(() =>
{
	var settings = new DotNetCorePackSettings
	{
		Configuration = build.Configuration,
		VersionSuffix = build.Version.Suffix,
		IncludeSymbols = true,
		OutputDirectory = "./artifacts/packages"
	};
	foreach (var project in build.ProjectFiles)
	{
		DotNetCorePack(project.FullPath, settings);
	}
});

Task("Publish")
	.Does(() =>
{
	var settings = new NuGetPushSettings 
	{
		ApiKey = "zUyzLYZFKIbse+1D4CtQZQ==",
		Source = "http://10.1.7.158/nuget/default/"
	};

	var packages = System.IO.Directory.GetFiles("./artifacts/packages");
	foreach (var package in packages)
	{
		NuGetPush(package, settings);
	}
});

Task("Default")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Pack")
	.IsDependentOn("Publish")
	.Does(() =>
{
	util.PrintInfo();
});

Task("Version")
	.Does(() =>
{
	Information($"{build.FullVersion()}");
});

Task("Print")
	.Does(() =>
{
	util.PrintInfo();
});

RunTarget(target);
