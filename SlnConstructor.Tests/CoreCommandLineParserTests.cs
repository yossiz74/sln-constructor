using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlnConstructor.Tests
{
    [TestClass]
    public class CoreCommandLineParserTests
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
    }

}
