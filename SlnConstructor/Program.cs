using System;
using System.Diagnostics;

namespace SlnConstructor
{
    class Program
    {
        static void Main(string[] args)
        {
            // get directory to scan from command line
            SlnConstructor.Core.CommandLineParser parser = new SlnConstructor.Core.CommandLineParser();
            try
            {
                parser.ParseArgs(args);
            }
            catch (ArgumentNullException)
            {
                Usage();
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Console.Error.WriteLine("Unable to read: " + parser.dir);
            }
            finally
            {
                Console.Out.Write("Press Enter to quit");
                Console.ReadLine();
            }
            // scan directory for projects
            // construct the solution from projects
        }

        private static void Usage()
        {
            Console.Out.WriteLine("Usage: SlnConstructor.exe <dir>");
        }
    }
}
