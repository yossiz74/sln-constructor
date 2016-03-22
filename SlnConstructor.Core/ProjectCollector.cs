using System.IO;
using System.Collections.Specialized;
using Microsoft.Build.Construction;

namespace SlnConstructor.Core
{
    public class ProjectCollector
    {
        public StringCollection projects { get; internal set; }

        internal void ScanDirForProjects(string givenDir, string extension)
        {
            projects = new StringCollection();
            DirectoryInfo di = new DirectoryInfo(givenDir);
            foreach (var fi in di.EnumerateFiles("*." + extension,SearchOption.AllDirectories))
            {
                projects.Add(fi.FullName);
            }
        }

        public void ScanDirForProjects(string givenDir, string extension, string toolsVersion)
        {
            projects = new StringCollection();
            DirectoryInfo di = new DirectoryInfo(givenDir);
            foreach (var fi in di.EnumerateFiles("*." + extension, SearchOption.AllDirectories))
            {
                string path = fi.FullName;
                if (ProjectVersionMatch(path,toolsVersion))
                    projects.Add(path);
            }
        }

        internal bool ProjectVersionMatch(string path, string toolsVersion)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            var root = ProjectRootElement.Open(path);
            return (root.ToolsVersion == toolsVersion);
        }
    }
}