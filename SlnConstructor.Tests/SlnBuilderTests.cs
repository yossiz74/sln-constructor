using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SlnConstructor.Tests
{
    [TestClass]
    public class SlnBuilderTests
    {
        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void CreateSln_ExceptionIfSlnReadOnly()
        {
            string path = CreateTestSln();
            File.SetAttributes(path, FileAttributes.ReadOnly);
            Core.SlnBuilder sb = new Core.SlnBuilder();
            sb.WriteSolution(path);
        }
        [TestMethod]
        public void CreateSln_OverrideIfExists()
        {
            // arrange
            string path = CreateTestSln();
            Core.SlnBuilder sb = new Core.SlnBuilder();
            DateTime oldCreationTime = File.GetLastWriteTime(path);
            // act
            System.Threading.Thread.Sleep(50);
            sb.WriteSolution(path);
            // assert
            DateTime creationTime = File.GetLastWriteTime(path);
            Assert.IsFalse(DateTime.Compare(oldCreationTime,creationTime)==0);
        }
        [TestMethod]
        public void CreateSln_VerifyValidFormat()
        {
            // arrange
            string path = @"F:\Temp\test.sln";
            Core.SlnBuilder sb = new Core.SlnBuilder();
            // act
            sb.WriteSolution(path);
            // assert
            // TODO
            XmlTextReader reader = new XmlTextReader(path);
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element: // The node is an element.
                        Console.Write("<" + reader.Name);
                        Console.WriteLine(">");
                        break;
                    case XmlNodeType.Text: //Display the text in each element.
                        Console.WriteLine(reader.Value);
                        break;
                    case XmlNodeType.EndElement: //Display the end of the element.
                        Console.Write("</" + reader.Name);
                        Console.WriteLine(">");
                        break;
                }
            }
        }
        private string CreateTestSln()
        {
            string dir = "F:\\Temp";
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, "test.sln");
            CreateEmptySln(path);
            return path;
        }
        private void CreateEmptySln(string path)
        {
            if (File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }
            var slnFile = new XDocument(new XElement("Root"));
            slnFile.Save(path);
        }
    }
}
