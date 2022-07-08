using System.Collections.Generic;

namespace TestSolutionGenerator
{
    public interface IProjectGenerator
    {
        IReadOnlyList<string> GenerateProjects(string root);
    }
}