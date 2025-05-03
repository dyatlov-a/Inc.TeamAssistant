using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Renci.SshNet;
using static Nuke.Common.Tools.Docker.DockerTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
public sealed class Build : NukeBuild
{
    [Parameter("Docker hub username", Name = "hubusername")]
    private readonly string HubUsername = default!;

    [Parameter("Docker hub password", Name = "hubpassword")]
    private readonly string HubPassword = default!;

    [Parameter("Server name", Name = "servername")]
    private readonly string ServerName = default!;

    [Parameter("Server username", Name = "serverusername")]
    private readonly string ServerUsername = default!;

    [Parameter("Server password", Name = "serverpassword")]
    private readonly string ServerPassword = default!;
    
    public static int Main () => Execute<Build>(x => x.Compile);

    private AbsolutePath SourceDirectory => RootDirectory / "src";
    private AbsolutePath OutputDirectory => RootDirectory / "output";
    private AbsolutePath TestsDirectory => RootDirectory / "tests";
    private AbsolutePath TestReportsDirectory => OutputDirectory / "test-reports";

    private const string AppDirectory = "/home/teamassist/prod";
    private const string MigrationRunnerProject = "Inc.TeamAssistant.MigrationsRunner";
    private readonly string AppProject = "Inc.TeamAssistant.Gateway";
    private readonly string DesignProject = "Inc.TeamAssistant.Stories";

    private IEnumerable<string> ProjectsForPublish => [AppProject, DesignProject, MigrationRunnerProject];

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution]
    private readonly Solution Solution = default!;

    [GitVersion(Framework = "net9.0")]
    private readonly GitVersion GitVersion = default!;

    private Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            OutputDirectory.CreateOrCleanDirectory();
        });

    private Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    private Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    private Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .ResetVerbosity()
                .SetResultsDirectory(TestReportsDirectory)
                .When(c => IsServerBuild, c => c.EnableUseSourceLink())
                .EnableNoRestore()
                .EnableNoBuild()
                .CombineWith(Solution.GetAllProjects("*Tests"), (_, p) => _
                    .SetProjectFile(p)
                    .SetLoggers($"junit;LogFileName={p.Name}-results.xml;MethodFormat=Class;FailureBodyFormat=Verbose")));
        });

    private Target Publish => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetPublish(s => s
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .CombineWith(ProjectsForPublish, (ss, p) => ss
                    .SetProject(Solution.GetAllProjects(p).SingleOrDefault())
                    .SetOutput(OutputDirectory / p)));
        });

    private Target BuildImages => _ => _
        .DependsOn(Publish)
        .Executes(() =>
        {
            DockerBuild(x => x
                .DisableProcessOutputLogging()
                .SetProcessWorkingDirectory(RootDirectory)
                .SetPath(".")
                .SetFile("cicd/dockerfile.app_component")
                .SetBuildArg($"PROJECT={AppProject}")
                .SetTag(GetImageName(AppProject)));
            
            DockerBuild(x => x
                .DisableProcessOutputLogging()
                .SetProcessWorkingDirectory(RootDirectory)
                .SetPath(".")
                .SetFile("cicd/dockerfile.static_component")
                .SetBuildArg($"PROJECT={DesignProject}")
                .SetTag(GetImageName(DesignProject)));
            
            DockerBuild(x => x
                .DisableProcessOutputLogging()
                .SetProcessWorkingDirectory(RootDirectory)
                .SetPath(".")
                .SetFile("cicd/dockerfile.migrations_runner")
                .SetTag(GetImageName(MigrationRunnerProject)));
        });

    private Target PushImages => _ => _
        .DependsOn(BuildImages)
        .Executes(() =>
        {
            DockerLogin(s => s
                .SetUsername(HubUsername)
                .SetPassword(HubPassword)
                .DisableProcessOutputLogging());

            foreach (var appProject in ProjectsForPublish)
                DockerPush(s => s
                    .SetName(GetImageName(appProject))
                    .DisableProcessOutputLogging());
        });

    private Target Deploy => _ => _
        .DependsOn(PushImages)
        .Executes(() =>
        {
            using var client = new SshClient(ServerName, ServerUsername, ServerPassword);
            client.Connect();

            foreach (var appProject in ProjectsForPublish)
            {
                var image = GetImageName(appProject);
                
                client.RunCommand($"docker pull {image}");
                Console.WriteLine($"Image {image} pulled");
            }

            client.RunCommand($"cd {AppDirectory} && docker compose down");
            Console.WriteLine("App stopped");

            client.RunCommand($"cd {AppDirectory} && docker compose up -d");
            Console.WriteLine("App started");

            client.Disconnect();
        });

    private string GetImageName(string projectName) => $"dyatlovhome/{projectName.ToLowerInvariant()}:latest";
}
