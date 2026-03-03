using System;

namespace RPGMakerMVCheatMenuPatcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

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

                string input = File.ReadAllText(path);

                // The search text we want to replace with an actual newline
                string searchText =  "        <script type=\"text/javascript\" src=\"js/main.js\"></script>\n";
                string replaceText = "        <script type=\"text/javascript\" src=\"js/main.js\"></script>\n        <script type=\"text/javascript\" src=\"js/main2.js\"></script>\n";

                // Replace with the system's newline (Environment.NewLine works cross-platform)
                input = input.Replace(searchText, replaceText);

                searchText =  "        <script type=\"text/javascript\" src=\"www/js/main.js\"></script>\n";
                replaceText = "        <script type=\"text/javascript\" src=\"www/js/main.js\"></script>\n        <script type=\"text/javascript\" src=\"www/js/main2.js\"></script>\n";

                // Replace with the system's newline (Environment.NewLine works cross-platform)
                input = input.Replace(searchText, replaceText);

                File.WriteAllText(path, input);

                Console.WriteLine($"Patch path: {path} completed!");
            }

            Console.WriteLine("Press a key to close the application!");
            Console.ReadKey();
        }
    }
}
