using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace TestSolutionGenerator
{
    public class LinearFsharp : IProjectGenerator
    {
        private long _numberOfProjects;

        public LinearFsharp(int numberOfProjects)
        {
            _numberOfProjects = numberOfProjects;
        }
        
        public IReadOnlyList<string> GenerateProjects(string root)
        {
            var result = new List<string>();
            for (var i = 1; i <= _numberOfProjects; i++)
            {
                var path = CreateProject(root, i);
                result.Add(path);
            }

            var directoryBuildPropsPath = Path.Combine(root, Projects.DirectoryBuildProps);
            var rootElement = ProjectRootElement.Create(NewProjectFileOptions.None);
            rootElement.AddProperty(Projects.TargetFrameworks, Projects.NetStandard20);
            rootElement.Save(directoryBuildPropsPath);

            return result;
        }
        
        private string CreateProject(string root, int i)
        {
            var projectName = $"Lib{i:D3}";
            var projectRoot = Path.Combine(root, projectName);
            Directory.CreateDirectory(projectRoot);
            
            var projectPath = Path.Combine(root, projectName, $"{projectName}.fsproj");
            var rootElement = ProjectRootElement.Create(NewProjectFileOptions.None);
            rootElement.Sdk = Projects.MicrosoftNETSdk;
            
            if (i < _numberOfProjects)
            {
                var referencedName = $"Lib{(i + 1):D3}";
                var referencedProjectPath = Path.Combine("..", referencedName, $"{referencedName}.fsproj");
                rootElement.AddItem("ProjectReference", referencedProjectPath);
            }

            var code = Resources.GetResource("fsharp")
                .Replace("<namespace>", projectName);

            File.WriteAllText(Path.Combine(projectRoot, "MyModule.fs"), code);
            rootElement.AddItem("Compile", "MyModule.fs");

            rootElement.Save(projectPath);
            return projectPath;
        }
    }
}