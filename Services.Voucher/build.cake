#addin nuget:?package=Cake.FileHelpers&version=5.0.0

var version = Argument("version", "0.0.1");
var framework = Argument("f", "net5.0");;
var configuration = Argument("c", "Release");
var srcDirectory = Directory(".");
var coverageDirectory = Directory("./coverage");

Task("Clean").Does(() => {
  CleanDirectories(srcDirectory.Path + "/**/bin");
  CleanDirectories(srcDirectory.Path + "/**/obj");
  CleanDirectories(srcDirectory.Path + "/**/pkg");
  CleanDirectories(srcDirectory.Path + "/**/TestResults");
  CleanDirectory(coverageDirectory);
});

Task("Restore").Does(() => {
  foreach(var sln in GetSolutionFiles()) {
    DotNetRestore(sln.FullPath, new DotNetRestoreSettings {
      DisableParallel = true,
      Interactive = false,
      Force = true,
    });
  }
});

Task("Build").Does(() => {
  foreach(var sln in GetSolutionFiles()) {
    DotNetBuild(sln.FullPath, new DotNetBuildSettings {
      Configuration = configuration,
      Framework = framework,
      NoIncremental = true,
      NoRestore = true,
      NoLogo = true,
      Verbosity = DotNetVerbosity.Normal,
      ArgumentCustomization = args =>
        args.Append("/p:AssemblyVersion=" + version)
            .Append("/p:PackageVersion=" + version)
            .Append("/p:VersionPrefix=" + version)
            .Append("/m:1")
    });
  }
});

Task("Test").Does(() => {
  foreach(var sln in GetSolutionFiles()) {
    DotNetTest(sln.FullPath, new DotNetTestSettings {
      Configuration = configuration,
      Framework = framework,
      NoRestore = true,
      NoBuild = true,
      ArgumentCustomization = args => args
            .Append("--collect").AppendQuoted("XPlat Code Coverage")
            .Append("--logger").Append("trx")
    });
  }
});

Task("Report").Does(() => {
  // generate coverage report
  var files = GetFiles("./**/TestResults/*/coverage.cobertura.xml");

  // please, try to use ReportGenerator later. Currently, it does not work on linux
  StartProcess("dotnet", new ProcessSettings {
    WorkingDirectory = Environment.CurrentDirectory,
    Arguments = new ProcessArgumentBuilder()
      .Append("reportgenerator")
      .AppendQuoted("-reports:" + string.Join(";", files.Select(r => r.FullPath)))
      .AppendQuoted("-targetdir:" + coverageDirectory.Path)
      .AppendQuoted("-reporttypes:TextSummary;HtmlInline_AzurePipelines_Dark")
  });

  // print summaries to console
  var summaries = GetFiles($"{coverageDirectory}/Summary.txt");
  foreach(var summary in summaries) {
    var summaryText = FileReadText(summary);
    if(summaryText.Contains("Line coverage: 100.0%")) {
      Information(summaryText);
    } else {
      Error(summaryText);
      throw new Exception("The code coverage is not 100.0%.");
    }
  }
});

Task("Default")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Report");

RunTarget(Argument("target", "Default"));

FilePathCollection GetSolutionFiles() {
  return GetFiles(srcDirectory.Path + "/*.sln");
}
