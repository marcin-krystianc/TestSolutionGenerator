using System.IO;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace TestSolutionGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: ConsoleTheme.None)
                .CreateLogger();

            Log.Logger.Information("Hello World");

            var solutionName = "Linear300Fs";
            var solutionRoot = solutionName;
            if (Directory.Exists(solutionRoot))
                Directory.Delete(solutionRoot, true);

            var generator = new LinearFsharp(300);
            var projectPaths = generator.GenerateProjects(solutionRoot);
            await SolutionGenerator.GenerateSolution(solutionName, solutionRoot, projectPaths);
        }
    }
}
