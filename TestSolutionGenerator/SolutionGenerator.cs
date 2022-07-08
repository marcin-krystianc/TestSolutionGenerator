using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace TestSolutionGenerator;

public static class SolutionGenerator
{
    public static async Task GenerateSolution(string solutionName, string root, IEnumerable<string> paths)
    {
        var processRunner = new ProcessRunner(root);
        await processRunner.RunAsync("dotnet", "new", "sln", "-n", solutionName);
        
        var queue = new Queue<string>(paths);
        Log.Logger.Information($"Generating a solution '{solutionName}.sln' in the '{Path.GetFullPath(root)}' containing {queue.Count} projects.");
        
        // What is the command line length limit?
        // Theoretically 32k - https://devblogs.microsoft.com/oldnewthing/20031210-00/?p=41553
        // On some machines it fails if we use 32K limit :-(
        var cmdLengthLimit = 2000;
        var batch = new List<string>();
        var leftCharacters = cmdLengthLimit;
        while (queue.Any() || batch.Any())
        {
            if (queue.Any())
            {
                var projectPath = Path.GetRelativePath(root, queue.Peek());
                if (!batch.Any() || leftCharacters > projectPath.Length)
                {
                    queue.Dequeue();
                    batch.Add(projectPath);
                    leftCharacters -= projectPath.Length;
                    continue;
                }
            }

            if (batch.Any())
            {
                await processRunner.RunAsync("dotnet", new[] { "sln", "add", }.Concat(batch).ToArray());
                batch.Clear();
                leftCharacters = cmdLengthLimit;
            }
        }
    }
}