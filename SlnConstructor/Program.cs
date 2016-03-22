using System;
using System.IO;

namespace SlnConstructor
{
    class Program
    {
        static void Main(string[] args)
        {
            // get directory to scan from command line
            Core.CommandLineParser parser = new Core.CommandLineParser();
            try
            {
                parser.ParseArgs(args);
            }
            catch (ArgumentNullException)
            {
                Usage();
                Console.ReadLine();
                return;
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Console.Error.WriteLine("Unable to read: " + parser.dir);
                Console.ReadLine();
                return;
            }
            // scan directory for projects
            Core.ProjectCollector pc = new Core.ProjectCollector();
            try
            {
                pc.ScanDirForProjects(parser.dir, "csproj", "4.0");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.ReadLine();
                return;
            }
            if (pc.projects.Count == 0)
            {
                Console.Out.WriteLine("No project files found matching the search parameters");
                Console.ReadLine();
                return;
            }
            Console.Out.WriteLine("Found {0} project files",pc.projects.Count);
            // construct the solution from projects
            Core.SlnBuilder sb = new Core.SlnBuilder();
            sb.WriteSolution(Path.Combine(parser.dir,"general.sln"));
            // All done
            Console.ReadLine();
            return;
        }

        private static void Usage()
        {
            Console.Out.WriteLine("Usage: SlnConstructor.exe <dir>");
        }
    }
}
