using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileCopy
{
    class Program
    {
        // Process all files in the directory passed in, recurse on any directories
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);
 

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }
 

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            //Console.WriteLine("Processed file '{0}'.", path);
            if (Path.GetExtension(path) == ".pdf")
            {
                if (!FileList.Contains(path))
                {
                    FileList.Add(path);
                }
            }
        }
 

        static List<string> FileList = new List<string>();
 

        static void Main(string[] args)
        {
            //Console.WriteLine(Directory.GetCurrentDirectory().ToString());

            //string SourceDirectory = @"C:\CopyTestLocation";
            //string destinationFile = @"E:\CopyTestLocation\Temp\";
            string SourceDirectory = @"C:\";
            string destinationFile = @"E:\Temp\";
            if (!Directory.Exists(SourceDirectory))
            {
                Console.WriteLine("Cannot find E drive!");
                Console.ReadLine();
                Environment.Exit(0);
            }
            else if(!Directory.Exists(destinationFile))
            {
                Console.WriteLine("Cannot find temp folder!");
                Console.ReadLine();
                Environment.Exit(0);
            }

 
            string ChosenDir = null;
            DateTime NewestDateTime = DateTime.MinValue;
            foreach(string dir in Directory.GetDirectories(SourceDirectory))
            {
                DateTime TimeOfCreation =  Directory.GetCreationTime(dir.ToString());
                if (DateTime.Compare(TimeOfCreation, NewestDateTime) > 0)
                {
                    ChosenDir = dir;
                    NewestDateTime = TimeOfCreation;
                }
            }
            Console.WriteLine("Reading from: " + SourceDirectory);
            //Console.WriteLine(NewestDateTime.ToString());
            Console.WriteLine("Press Enter to use [" + Path.GetFileName(ChosenDir) + "] otherwise enter correct FolderName: ");
            string UserDiretory = Console.ReadLine();
            if (UserDiretory != "")
            {
                while (true)
                {
                    if (Directory.Exists(SourceDirectory + UserDiretory))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Folder Not Found. Please re-try: ");
                        UserDiretory = Console.ReadLine();
                    }
                }
            }
            Console.WriteLine("Pulling files from: " + UserDiretory);
            Console.WriteLine("Copying files to " + destinationFile);

 
            while (true)
            {
                ProcessDirectory(SourceDirectory + UserDiretory);
                foreach (string s in FileList)
                {
                    string NameOfFile = Path.GetFileName(s);
                    if (!File.Exists(destinationFile + NameOfFile))
                    {
                        Console.WriteLine(s + " -> " + destinationFile);
                        File.Copy(s, destinationFile + NameOfFile);
                    }
                }
            }
        }
    }
}