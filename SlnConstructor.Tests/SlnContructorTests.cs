using System;
using System.IO;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Build.Construction;

namespace SlnConstructor.Tests
{
    [TestClass]
    public class SlnContructorTests
    {
        [TestMethod]
        public void CommandLineParser_ReturnCorrectDir()
        {
            // set up
            string givenDir = "TestProjects";
            Directory.CreateDirectory(givenDir);
            string[] args = { givenDir };
            Core.CommandLineParser parser = new Core.CommandLineParser();
            // run
            parser.ParseArgs(args);
            // check pass/fail criteria
            Assert.AreEqual(parser.dir, givenDir);
            // tear down
            Directory.Delete(givenDir,true);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CommandLineParser_ExceptionIfNoDirProvided()
        {
            string[] args = { };
            Core.CommandLineParser parser = new Core.CommandLineParser();
            parser.ParseArgs(args);
        }
        [TestMethod]
        [ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
        public void CommandLineParser_ExceptionIfDirNotFound()
        {
            string givenDir = "projects";
            string[] args = { givenDir };
            Core.CommandLineParser parser = new Core.CommandLineParser();
            parser.ParseArgs(args);
        }
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
                CreateEmptyProject(prj,"14.0");
            }
            Core.ProjectCollector pc = new Core.ProjectCollector();
            // run
            pc.ScanDirForProjects(givenDir, "*proj");
            // check pass/fail criteria
            CollectionAssert.AreEquivalent(projects, pc.projects);
            // tear down
            Directory.Delete(givenDir,true);
        }
        [TestMethod]
        public void ProjectCollector_FindOnlyVS2015Projects()
        {
            // set up
            string givenDir = @"F:\Temp\TestProjects";
            if (Directory.Exists(givenDir)) Directory.Delete(givenDir, true);
            StringCollection projects = new StringCollection();
            projects.Add(Path.Combine(givenDir, "Proj2015.csproj"));
            CreateEmptyProject(Path.Combine(givenDir, "Proj2015.csproj"),"14.0");
            CreateEmptyProject(Path.Combine(givenDir, "Proj2012.csproj"), "12.0");
            Core.ProjectCollector pc = new Core.ProjectCollector();
            // run
            pc.ScanDirForProjects(givenDir, "csproj","14.0");
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
            string path = Path.Combine(givenDir, "Proj2015.csproj");
            CreateEmptyProject(path, "12.0");
            Core.ProjectCollector pc = new Core.ProjectCollector();
            //
            bool res = pc.ProjectVersionMatch(path, "14.0");
            //
            Assert.IsFalse(res);
        }
        // TODO: test exception if project is not well-formed
        private void CreateEmptyProject(string path,string version)
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
