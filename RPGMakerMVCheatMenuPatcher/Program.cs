using System;

namespace RPGMakerMVCheatMenuPatcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;

    class Program
    {
        static string GetParentDir(string path)
        {
            if (path == null)
            {
                return null;
            }

            try
            {
                string parentDir = Directory.GetParent(path).FullName;
                return parentDir;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        static string[] SearchFiles(string parentDir, string search)
        {
            string[] files = null;

            List<string> paths = new List<string>();

            IEnumerable<string> enumerable = Directory.EnumerateFiles(parentDir, search, new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            });

            foreach (string path in enumerable)
            {
                paths.Add(path);
            }

            files = paths.ToArray();

            return files;
        }

        static string[] getGamePaths()
        {
            string currentDir = Directory.GetCurrentDirectory();
            return getGamePaths(currentDir);
        }

        static string[] getGamePaths(string currentDir)
        {
            if (currentDir == null)
                return null;

            string parentDir = null;
            string[] files = null;

            List<string> filesFounded = new List<string>();

            int trys = 0;
            int maxTrys = 0;

            do
            {
                //parentDir = GetParentDir(currentDir);

                parentDir = null;
    
            if (parentDir != null)
                    currentDir = parentDir;
                else
                    parentDir = currentDir;

                files = SearchFiles(parentDir, "index.html");
                if (files != null && files.Length > 0)
                {
                    foreach (string file in files)
                    {
                        if (filesFounded.Contains(file))
                            continue;
                        filesFounded.Add(file);
                    }
                }
                trys++;
            } while (trys < maxTrys);

            files = filesFounded.ToArray();

            return files;
        }

        static string GetExecutableDirectory()
        {
            return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        }

        static void Main()
        {
            Console.WriteLine("Please wait, it takes a while to find patch paths");
            string[] paths = getGamePaths();

            if (paths == null)
            {
                return;
            }

            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];

                if (path == null)
                {
                    continue;
                }

                Console.WriteLine($"Found Patch path: {path}");

                Console.WriteLine($"Patch Path: {path}");

                string[] lines = File.ReadAllLines(path);

                string all = "";

                foreach (string line in lines)
                {
                    string input = line;

                    // The search text we want to replace with an actual newline
                    string main = "<script type=\"text/javascript\"\\s+src=\"js/main\\.js\"></script>";
                    string searchText = @$"^(\s+)?{main}";
                    string replaceText = "$1<script type=\"text/javascript\" src=\"js/main.js\"></script>\n$1<script type=\"text/javascript\" src=\"js/main2.js\"></script>";

                    input = Regex.Replace(input, searchText, replaceText);

                    main = "<script type=\"text/javascript\"\\s+src=\"www/js/main\\.js\"></script>";
                    searchText = @$"^(\s+)?{main}";
                    replaceText = "$1<script type=\"text/javascript\" src=\"www/js/main.js\"></script>\n$1<script type=\"text/javascript\" src=\"www/js/main2.js\"></script>";

                    input = Regex.Replace(input, searchText, replaceText);

                    all += input + "\n";
                }

                File.WriteAllText(path, all);

                Console.WriteLine($"Patch path: {path} completed!");
            }

            Console.WriteLine("Press a key to close the application!");
            Console.ReadKey();
        }
    }
}
