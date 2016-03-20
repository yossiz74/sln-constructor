using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            System.IO.Directory.CreateDirectory(givenDir);
            string[] args = { givenDir };
            // run
            SlnConstructor.Core.CommandLineParser parser = new SlnConstructor.Core.CommandLineParser();
            parser.ParseArgs(args);
            // check pass/fail criteria
            Assert.AreEqual(parser.dir, givenDir);
            // tear down
            System.IO.Directory.Delete(givenDir);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CommandLineParser_ExceptionIfNoDirProvided()
        {
            string[] args = { };
            SlnConstructor.Core.CommandLineParser parser = new SlnConstructor.Core.CommandLineParser();
            parser.ParseArgs(args);
        }
        [TestMethod]
        [ExpectedException(typeof(System.IO.DirectoryNotFoundException))]
        public void CommandLineParser_ExceptionIfDirNotFound()
        {
            string givenDir = "projects";
            string[] args = { givenDir };
            SlnConstructor.Core.CommandLineParser parser = new SlnConstructor.Core.CommandLineParser();
            parser.ParseArgs(args);
        }
    }

}
