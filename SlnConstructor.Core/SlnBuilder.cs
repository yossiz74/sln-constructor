using System.IO;
using System.Xml.Linq;

namespace SlnConstructor.Core
{
    public class SlnBuilder
    {
        public void WriteSolution(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var slnFile = new XDocument(new XElement("Root"));
            slnFile.Save(path);
        }
    }
}
