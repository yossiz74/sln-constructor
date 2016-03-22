using System.IO;
using System.Collections.Specialized;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlnConstructor.Tests
{
    [TestClass]
    public class CoreProjectCollectorTests
    {
        [TestMethod]
        public void ProjectCollector_FindAllProjects()
        {
            // set up
            string givenDir = @"F:\Temp\TestProjects";
            if (Directory.Exists(givenDir)) Directory.Delete(givenDir, true);
            StringCollection projects = new StringCollection();
            projects.Add(givenDir + "\\Proj0.vcxproj");
            projects.Add(givenDir + "\\Proj1\\Proj1a.vcxproj");
            projects.Add(givenDir + "\\Proj1\\Proj1b.csproj");
            projects.Add(givenDir + "\\Other\\Proj3\\Proj3.vcxproj");
            projects.Add(givenDir + "\\Other\\MyProj\\MyOtherProj.csproj");
            foreach (string prj in projects)
            {
                CreateEmptyProject(prj, "14.0");
            }
            Core.ProjectCollector pc = new Core.ProjectCollector();
            // run
            pc.ScanDirForProjects(givenDir, "*proj");
            // check pass/fail criteria
            CollectionAssert.AreEquivalent(projects, pc.projects);
            // tear down
            Directory.Delete(givenDir, true);
        }
        [TestMethod]
        public void ProjectCollector_FindOnlyVS2015Projects()
        {
            // set up
            string givenDir = @"F:\Temp\TestProjects";
            if (Directory.Exists(givenDir)) Directory.Delete(givenDir, true);
            StringCollection projects = new StringCollection();
            projects.Add(Path.Combine(givenDir, "Proj2015.csproj"));
            CreateEmptyProject(Path.Combine(givenDir, "Proj2015.csproj"), "14.0");
            CreateEmptyProject(Path.Combine(givenDir, "Proj2012.csproj"), "12.0");
            Core.ProjectCollector pc = new Core.ProjectCollector();
            // run
            pc.ScanDirForProjects(givenDir, "csproj", "14.0");
            // check pass/fail criteria
            CollectionAssert.AreEquivalent(projects, pc.projects);
            // tear down
            Directory.Delete(givenDir, true);
        }
        [TestMethod]
        public void ProjectCollector_ProjectVersionMatch()
        {
            string givenDir = @"F:\Temp\TestProjects";
            if (Directory.Exists(givenDir)) Directory.Delete(givenDir, true);
            string path = Path.Combine(givenDir, "Proj2015.csproj");
            CreateEmptyProject(path, "14.0");
            Core.ProjectCollector pc = new Core.ProjectCollector();
            //
            bool res = pc.ProjectVersionMatch(path, "14.0");
            //
            Assert.IsTrue(res);
        }
        [TestMethod]
        public void ProjectCollector_ProjectVersionMismatch()
        {
            //
            string givenDir = @"F:\Temp\TestProjects";
            if (Directory.Exists(givenDir)) Directory.Delete(givenDir, true);
            string path = Path.Combine(givenDir, "Proj2012.csproj");
            CreateEmptyProject(path, "12.0");
            Core.ProjectCollector pc = new Core.ProjectCollector();
            //
            bool res = pc.ProjectVersionMatch(path, "14.0");
            //
            Assert.IsFalse(res);
        }
        [TestMethod]
        [ExpectedException(typeof(Microsoft.Build.Exceptions.InvalidProjectFileException))]
        public void ProjectCollector_ExceptionIfProjectFileMalformed()
        {
            string givenDir = @"F:\Temp\BadProjects";
            if (Directory.Exists(givenDir)) Directory.Delete(givenDir, true);
            string path = Path.Combine(givenDir, "Proj2015.csproj");
            Core.ProjectCollector pc = new Core.ProjectCollector();
            Directory.CreateDirectory(givenDir);
            using (TextWriter fs = File.CreateText(path))
                fs.Write("not a csproj");
            pc.ProjectVersionMatch(path, "14.0");
        }
        [TestMethod]
        [ExpectedException(typeof(System.IO.FileNotFoundException))]
        public void ProjectCollector_ExceptionIfProjectFileMissing()
        {
            string givenDir = @"F:\Temp\TestProjects";
            if (Directory.Exists(givenDir)) Directory.Delete(givenDir, true);
            string path = Path.Combine(givenDir, "Proj2015.csproj");
            Core.ProjectCollector pc = new Core.ProjectCollector();
            Directory.CreateDirectory(givenDir);
            pc.ProjectVersionMatch(path, "14.0");
        }
        private void CreateEmptyProject(string path, string version)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var root = ProjectRootElement.Create();
            root.ToolsVersion = version;
            var group = root.AddPropertyGroup();
            group.AddProperty("Configuration", "Debug");
            group.AddProperty("Platform", "x64");

            // references
            AddItems(root, "Reference", "System", "System.Core");

            // items to compile
            AddItems(root, "Compile", "test.cs");

            var target = root.AddTarget("Build");
            var task = target.AddTask("Csc");
            task.SetParameter("Sources", "@(Compile)");
            task.SetParameter("OutputAssembly", "test.dll");

            root.Save(path);
        }
        private void AddItems(ProjectRootElement elem, string groupName, params string[] items)
        {
            var group = elem.AddItemGroup();
            foreach (var item in items)
            {
                group.AddItem(groupName, item);
            }
        }

    }
}
