using System;

namespace SlnConstructor.Core
{
    public class CommandLineParser
    {
        //private string[] args;
        private string Dir = "";

        public void ParseArgs(string[] args)
        {
            if (args.GetLength(0) == 0)
                throw new ArgumentNullException();
            dir = args[0];
        }

        public string dir
        {
            get
            {
                return Dir;
            }
            internal set
            {
                Dir = value; // so it could be retrieved by the caller to display an appropriate message
                if (!System.IO.Directory.Exists(Dir))
                    throw new System.IO.DirectoryNotFoundException();
            }
        }
    }
}